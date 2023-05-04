#include <ESP8266WiFi.h>
#include <ESP8266WebServer.h>
#include <arduino-timer.h>

enum MODES {
  NONE,
  MANUAL,
  TIMER,
  SENSOR
};

const char* wifiName = "wetme"; // Change device wifi name
const int sensorPin = A0;
const int pumpPin = D1;
const int ledPin = LED_BUILTIN;

const int TURN_ON = LOW;
const int TURN_OFF = HIGH;

IPAddress localIp(192, 168, 1, 111);
IPAddress gateway(192, 168, 1, 1);
IPAddress subnet(255, 255, 0, 0);

MODES mode;
ESP8266WebServer server(80);

int waterLevel = 0;
int lowWaterLevel = 300; // Adjust low water sensitivity
bool timeBomb = false; // activate the timer pump
bool timeDoom = false; // activate the timer light
bool timerWatering = false;
bool hasLight = false;
auto timer = timer_create_default();

bool activatePump(void*) {
  if (!timerWatering) {
    timeBomb = true;
    Serial.println("Activating pump...");
  }

  return true;
}

bool activateLed(void*) {
  // Serial.println("Activating led...");

  timeDoom = true;
  return true;
}

bool stopTimerWater(void*) {
  timerWatering = false;
  digitalWrite(pumpPin, TURN_OFF);
  return false;
}

void reset() {
  timer.cancel();
  delay(100);

  lowWaterLevel = 300;

  timeBomb = false;
  timeDoom = false;
  hasLight = false;
  waterLevel = lowWaterLevel + 5;
  mode = NONE;

  digitalWrite(ledPin, TURN_OFF);
  digitalWrite(pumpPin, TURN_OFF);
}

void handleHomePage() {
  // Serial.println("Serving home page");

  String currentMode = "None";
  switch (mode) {
    case MANUAL:
      currentMode = "Manual";
      break;
    case TIMER:
      currentMode = "Timer";
      break;
    case SENSOR:
      currentMode = "Sensor";
      break;
  }

  server.send(200, "text/html", 
    "<div>"
    "<h1>"+ String(wifiName) +" Server</h1>"
    "<p>Water Level: "+ String(waterLevel) +"</p>"
    "<p>Water Status: "+ ((waterLevel > lowWaterLevel) ? "High" : "Low") +"</p>"
    "<p>Mode: "+ currentMode +"</p>"
    "</div>"
  );
}

void handleNotFound() {
  Serial.println("Serving not found page");

  server.send(404, "text/plain", "404: Not Found");
}

void handleManualMode() {
  Serial.println("Manual mode activated");

  reset();
  mode = MANUAL;

  timer.every(3000, activateLed);
  server.send(200, "text/plain", "MODE: manual");
}

void handleTimerMode() {
  Serial.println("Timer mode activated");

  if (server.hasArg("timeout") && server.arg("timeout") != NULL && server.arg("timeout").toInt() >= 1000) {
    reset();
    mode = TIMER;

    int timeout = server.arg("timeout").toInt();

    timer.every(timeout, activatePump);
    timer.every(1500, activateLed);
    server.send(200, "text/plain", "MODE: timer");
  } else {
    server.send(500, "text/plain", "ERROR: Invalid timeout value");
  }
}

void handleSensorMode() {
  Serial.println("Sensor mode activated");

  if (server.hasArg("lowLevel") && server.arg("lowLevel") != NULL && server.arg("lowLevel").toInt() >= 50) {
    reset();
    lowWaterLevel = server.arg("lowLevel").toInt();
    mode = SENSOR;

    timer.every(500, activateLed);
    server.send(200, "text/plain", "MODE: sensor");
  } else {
    server.send(500, "text/plain", "ERROR: Invalid low level value");
  }
}

void handleAskWaterLevel() {
  // Serial.println("Serving water level value");

  server.send(200, "text/plain", String(waterLevel));
}

void handleAskMode() {
  String currentMode = "None";
  switch (mode) {
    case MANUAL:
      currentMode = "Manual";
      break;
    case TIMER:
      currentMode = "Timer";
      break;
    case SENSOR:
      currentMode = "Sensor";
      break;
  }
  
  server.send(200, "text/plain", currentMode);
}

void handleOff() {
  reset();
  
  server.send(200, "text/plain", "MODE: none");
}

void doPump() {
  Serial.println("Level: " + String(waterLevel) + " / " + String(lowWaterLevel));

  if (mode == TIMER) {
    digitalWrite(pumpPin, TURN_ON);
    timerWatering = true;
    timer.every(5000, stopTimerWater);
  } else if (mode == MANUAL) {
    digitalWrite(pumpPin, TURN_ON);
  } else {
    if (waterLevel > lowWaterLevel) {
      Serial.println("WATER: HIGH");
      digitalWrite(pumpPin, TURN_OFF);
    } else {
      Serial.println("WATER: LOW");
      digitalWrite(pumpPin, TURN_ON);
    }
  }
}

void doLight() {
  if (hasLight) {
    // Serial.println("Turning light: OFF");
    digitalWrite(ledPin, TURN_OFF);
    hasLight = false;
  } else {
    // Serial.println("Turning light: ON");
    digitalWrite(ledPin, TURN_ON);
    hasLight = true;
  }
}

void setup() {
  Serial.begin(9600);
  Serial.println("Hello World");

  // Initialize Pin
  pinMode(ledPin, OUTPUT);
  pinMode(pumpPin, OUTPUT);

  // Initialize wifi
  WiFi.mode(WIFI_AP);
  WiFi.softAPConfig(localIp, gateway, subnet);
  WiFi.softAP(wifiName);

  // Initialize webserver
  server.on("/", HTTP_GET, handleHomePage);
  server.on("/manual", HTTP_POST, handleManualMode);
  server.on("/timer", HTTP_POST, handleTimerMode);
  server.on("/sensor", HTTP_POST, handleSensorMode);
  server.on("/water", HTTP_GET, handleAskWaterLevel);
  server.on("/off", HTTP_POST, handleOff);
  server.on("/mode", HTTP_GET, handleAskMode);
  server.onNotFound(handleNotFound);
  server.begin();

  reset();
  Serial.println("Initialization Done!");
}

void loop() {
  server.handleClient();
  timer.tick();

  int sensorValue = analogRead(sensorPin);
  waterLevel = (sensorValue * -1) + 1024;

  switch (mode) {
    case MANUAL:
      doPump();
      break;
    case TIMER:
      if (timeBomb) {
        doPump();
        timeBomb = false;
      }
      break;
    case SENSOR:
      doPump();
      break;
  }

  if (timeDoom) {
    doLight();
    timeDoom = false;
  }

  delay(100);
}