import express from "express";
import { faker } from "@faker-js/faker"

let lowWaterLevel = 300;
const maxWaterLevel = 1024;

const app = express();

app.use((req, res, next) => {
    console.log("Serving " + req.url);
    next();
})

app.get("/", (req, res) => {
    res.send("This is a test")
})

app.post("/manual", (req, res) => {
    res.send("MODE: manual")
})

app.post("/timer", (req, res) => {
    res.send("MODE: timer")
})

app.post("/sensor", (req, res) => {
    res.send("MODE: sensor")
})

app.get("/water", (req, res) => {
    res.send(faker.number.int(maxWaterLevel).toString())
})

app.post("/off", (req, res) => {
    res.send("MODE: none")
})

app.get("/mode", (req, res) => {
    res.send(faker.helpers.arrayElement(["None", "Manual", "Timer", "Sensor"]))
})

app.listen(80, () => {
    console.log("Sample app running...")
});