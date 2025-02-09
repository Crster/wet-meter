1.1 Manual Mode
 - Call Reset Function
     > Cancel all timer
     > Reset device variable
     > Turn off led ligth
     > Turn off pump

 - Set device to manual mode
     > Turn on pump

1.2 Timer Mode
 - Call Reset Function
     > Cancel all timer
     > Reset device variable
     > Turn off led ligth
     > Turn off pump

 - Set device to timer mode
     > Activate timer based on interval value

 - On interval activated
     > Turn on pump
     > Turn off pump after 5 seconds

1.3 Sensor Mode
 - Call Reset Function
     > Cancel all timer
     > Reset device variable
     > Turn off led ligth
     > Turn off pump

 - Set device to timer mode
     > Set low water level
     > Get sensor data
     > If sensor data is lower than water level
        > Turn on pump

2.1 Manual mode report input
  - On page load
	> Read history from manual.log

  - On enable manual mode
	> Write history to manual.log

  - On stop manual mode
      > Write history to manual.log

  - On water level status change
      > Write history to sensor.log

2.2 Timer mode report input
  - On page load
	> Read history from timer.log

  - On enable Timer mode
	> Write history to timer.log

  - On water level status change
      > Write history to sensor.log

2.3 Sensor mode report input
  - On page load
	> Read history from sensor.log

  - On enable sensor mode
	> Write history to sensor.log

  - On water level status change
      > Write history to sensor.log