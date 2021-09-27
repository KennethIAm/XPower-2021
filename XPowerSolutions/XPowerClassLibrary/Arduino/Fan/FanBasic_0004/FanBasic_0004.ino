#include <SPI.h>        //Libraries required to communicate with the arduino ethernet shield
#include <WiFi101.h>
#include <ArduinoHttpClient.h>
#include <Servo.h>  
#include "arduino_secrets.h"

String UNIT_ID = SECRET_DEVICE_ID;
String DEVICE_TYPE_ID = "4";
char serverAddress[] = SECRET_API_ROOT;  // server address
int port = 80;
int ERROR_PIN = 10;
char ssid[] = SECRET_WIFI_SSID;
char pass[] = SECRET_WIFI_PASS;
bool servoActivate = true;
bool shouldRun = false;

bool debugMode = false;

#define VT_PIN A0
#define AT_PIN A1

// Servo functionality specific to window
Servo myservo;  // create servo object to control a servo
int FAN_MIN_VALUE = 1;
int FAN_MAX_VALUE = 180;
int CURRENT_WINDOW_POS;

// Wifi properties
WiFiClient wifi_client;
String DEVICE_IP;

// Websocket functions.
WiFiServer server(80);  //Server port    
WiFiClient client;

// API functionality
HttpClient httpClient = HttpClient(wifi_client, serverAddress, port); 
bool apiConnectionError = false;  // Holds the current errorstatus of the api connection.
String readString;                //HTTP request read.
String deviceOnlineEndpoint = "/devices/IAmOnline";

void setup() {
 // Open serial monitor and wait for port to open
  if(debugMode){
     Serial.begin(9600);
     while (!Serial) {
      ; // wait for serial port to connect. Needed for Leonardo only
    }
  }
 


  // Run device specific setup
  device_setup();

  // Connect to Wifi.
  connect_to_wifi(ssid, pass);
  // Send startup message to API.
  call_startup_api();
}


void loop() {
  if(apiConnectionError == true){
    display_error_mode();
    return;
  }

  if(shouldRun){
    if(servoActivate){
        CURRENT_WINDOW_POS = FAN_MIN_VALUE;
    
        while (CURRENT_WINDOW_POS <= FAN_MAX_VALUE){
          myservo.write(CURRENT_WINDOW_POS);
          CURRENT_WINDOW_POS = CURRENT_WINDOW_POS + 1;
          delay(5);
        }
        servoActivate = false;
    }
    else{
      CURRENT_WINDOW_POS = FAN_MAX_VALUE;
    
        while (CURRENT_WINDOW_POS <= FAN_MIN_VALUE){
          myservo.write(CURRENT_WINDOW_POS);
          CURRENT_WINDOW_POS = CURRENT_WINDOW_POS - 1;
          delay(5);
        }
        servoActivate = true;
    }
  }
  
  
  run_websocket();

  
}

void device_setup(){
  myservo.attach(10);  // attaches the servo on pin 10 to the servo object
  myservo.write(FAN_MIN_VALUE);
}

void run_function_0(){
  client.println("HTTP/1.1 200 OK");
  client.println("Content-Type: application/json;charset=utf-8");
  client.println("Server: Arduino");
  client.println("Connection: close");
  client.println();
  client.println("{\"HasReturnInfo\": true,\"CurrentUsage\":" + String(ShowKwh()) + " }");
  client.println();
  
}

void run_function_1(){

  client.println("HTTP/1.1 200 OK");
  client.println("Content-Type: application/json;charset=utf-8");
  client.println("Server: Arduino");
  client.println("Connection: close");
  client.println();
  client.println("{\"HasReturnInfo\": false,\"CurrentUsage\":0}");
  client.println();

  shouldRun = true;
}

void run_function_2(){

  client.println("HTTP/1.1 200 OK");
  client.println("Content-Type: application/json;charset=utf-8");
  client.println("Server: Arduino");
  client.println("Connection: close");
  client.println();
  client.println("{\"HasReturnInfo\": false,\"CurrentUsage\":0}");
  client.println();

  shouldRun = false;
}





// WiFi functionality

// Connects to wifi and displays the wifi message in console.
void connect_to_wifi(char ssid[], char pass[]) {
  int wifi_status = WL_IDLE_STATUS;
  
  Serial.println("Attempting to connect to WPA Network...");
  Serial.println("Connecting to SSID : " + String(ssid));
  wifi_status = WiFi.begin(ssid, pass);   // Get status of the wifi.

  if ( wifi_status != WL_CONNECTED ) {
    Serial.println("Couldn't connect to selected WPA Network.");
    return;
  }

  Serial.println("Connected to " + String(ssid) + " WiFi!");

  // print your WiFi shield's IP address:
  IPAddress ip = WiFi.localIP();
  Serial.print("IP Address: ");
  Serial.println(ip);
  server.begin();  

  DEVICE_IP = IpAddress2String(ip);

}

// Converts IPAddress object to string.
String IpAddress2String(const IPAddress& ipAddress)
{
    return String(ipAddress[0]) + String(".") +
           String(ipAddress[1]) + String(".") +
           String(ipAddress[2]) + String(".") +
           String(ipAddress[3]);
}

// Display error, by blinking the error lamp.
void display_error_mode(){
  if(apiConnectionError == true)
  {
    if(digitalRead(ERROR_PIN) == HIGH){
      digitalWrite(ERROR_PIN, LOW);
    }
    else{
      digitalWrite(ERROR_PIN, HIGH);
    }
    delay(1000);
  }
}


// Connect and send startup message to API
void call_startup_api(){
    apiConnectionError = false;
  Serial.println("making API request");

  String contentType = "application/json; charset=UTF-8";
  String postData = "DeviceTypeId=" + DEVICE_TYPE_ID + "&UniqueDeviceIdentifier=" + UNIT_ID + "&IPAddress=" + DEVICE_IP;
  String connectionEndpoint = deviceOnlineEndpoint;
  String postStringComplete = connectionEndpoint + "?" + postData;
  Serial.println("Trying to reach URL: " + postStringComplete);
  
  httpClient.get(postStringComplete);

  // read the status code and body of the response
  int statusCode = httpClient.responseStatusCode();

  Serial.print("Status code: ");
  Serial.println(statusCode);
  
  if(statusCode != 200 && statusCode != 203){  
    apiConnectionError = true;
  }

  String response = httpClient.responseBody();
  
  Serial.print("Response: ");
  Serial.println(response);
}



// Websocket functionality
void run_websocket(){
  // Check for client request
  client = server.available();
  
  if (client) {                    //If client request arrived read the request
    while (client.connected()) {   
      if (client.available()) {
        char c = client.read();
     
        if (readString.length() < 100) {  
          readString += c;  
         }

          // Server WEB PAGE if requested by user      
          if (c == '\n') {          
            //Serial.println(readString); 
            //html file 

            //Translate the user request and check to switch on or off the fan
           if (readString.indexOf("?function0") >0){
                Serial.println("Function 1 has been called.");
                run_function_0();
           }
           else if (readString.indexOf("?function1") >0){
                Serial.println("Function 1 has been called.");
                run_function_1();
           }
           else if (readString.indexOf("?function2") >0){
               Serial.println("Function 2 has been called.");
               run_function_2();
           }
           else if(readString == "" || readString == "/"){
              client.println("<!DOCTYPE HTML>");
              
              client.println("<html>");
              client.println("<h1>You have reached unit: " + UNIT_ID + "</h1>");
              client.println("<h1>IP: ");
              client.println(DEVICE_IP);
              client.println("</h1>");
              client.println("</html>");  
           }
           else{
              Serial.println("Bad Request - Unkown command called: " + readString);
              //client.println("HTTP/1.1 400 Bad Request");     

              client.println("HTTP/1.1 400 Bad Request");
              client.println("Content-Type: application/json;charset=utf-8");
              client.println("Server: Arduino");
              client.println("Connection: close");
              
           }

            //clearing string for next read
            readString="";  

           delay(1);
           //stopping client
           client.stop();
           
         }
       }
    }
  }
}

float ShowKwh(){
  int vt_read = analogRead(VT_PIN);
  int at_read = analogRead(AT_PIN);

 

  float voltage = vt_read * (5.0 / 1024.0) * 5;
  float current = at_read * (5.0 / 1024.0) * 2.0;
  float watts = voltage * current;

 

  // Find calculation for Kwh

 

  return watts;
}
