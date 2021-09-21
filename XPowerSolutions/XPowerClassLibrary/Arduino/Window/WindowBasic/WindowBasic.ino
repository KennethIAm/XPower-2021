#include <SPI.h>        //Libraries required to communicate with the arduino ethernet shield
#include <WiFi101.h>
#include <ArduinoHttpClient.h>
#include <Servo.h>  
#include "arduino_secrets.h"

String UNIT_ID = SECRET_DEVICE_ID;
String DEVICE_TYPE_ID = "3";
char serverAddress[] = SECRET_API_ROOT;  // server address
int port = 80;
int ERROR_PIN = 10;
char ssid[] = SECRET_WIFI_SSID;
char pass[] = SECRET_WIFI_PASS;

// Servo functionality specific to window
Servo myservo;  // create servo object to control a servo
byte OPEN_WINDOW_VALUE = 180;
byte CLOSED_WINDOW_VALUE = 0;

// Wifi properties
WiFiClient wifi_client;
String DEVICE_IP;

// Websocket functions.
WiFiServer server(80);  //Server port    


// API functionality
HttpClient httpClient = HttpClient(wifi_client, serverAddress, port); 
bool apiConnectionError = false;  // Holds the current errorstatus of the api connection.
String readString;                //HTTP request read.
String deviceOnlineEndpoint = "/devices/IAmOnline";

void setup() {
 // Open serial monitor and wait for port to open
  Serial.begin(9600);
   while (!Serial) {
    ; // wait for serial port to connect. Needed for Leonardo only
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
  
  run_websocket();
}

void device_setup(){
  myservo.attach(10);  // attaches the servo on pin 9 to the servo object
  pinMode(ERROR_PIN, OUTPUT);
}

void run_function_0(){
  
}

void run_function_1(){
  myservo.write(OPEN_WINDOW_VALUE);
}

void run_function_2(){
  myservo.write(CLOSED_WINDOW_VALUE);
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
  WiFiClient client = server.available();
  
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

                String s = "HTTP/1.1 200 OK\r\nContent-Type: text/html\r\n\r\n<!DOCTYPE HTML>\r\n<html>\r\nFUNCTION0 RUN";
                s += "</html>\n";
                
                client.print(s);
           }
           
           //Translate the user request and check to switch on or off the fan
           if (readString.indexOf("?function1") >0){
                Serial.println("Function 1 has been called.");
                run_function_1();

                String s = "HTTP/1.1 200 OK\r\nContent-Type: text/html\r\n\r\n<!DOCTYPE HTML>\r\n<html>\r\nFUNCTION1 RUN";
                s += "</html>\n";
                
                
                client.print(s);
               
               //client.println("HTTP/1.1 204 OK"); 
           }
           else if (readString.indexOf("?function2") >0){
               Serial.println("Function 2 has been called.");
               run_function_2();
               //client.println("HTTP/1.1 204 OK"); 

              
                
                String s = "HTTP/1.1 200 OK\r\nContent-Type: text/html\r\n\r\n<!DOCTYPE HTML>\r\n<html>\r\nFUNCTION2 RUN";
                s += "</html>\n";
                client.print(s);
               
               
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

              String s = "HTTP/1.1 400 Bad Request\r\nContent-Type: text/html\r\n\r\n<!DOCTYPE HTML>\r\n<html>\r\nUnkown command";
              s += "</html>\n";
              client.print(s);
              
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
