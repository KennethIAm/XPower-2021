#include <SPI.h>        //Libraries required to communicate with the arduino ethernet shield
#include <WiFi101.h>
#include <ArduinoHttpClient.h>

String UNIT_ID = "0001";
IPAddress DEVICE_IP;
char ssid[] = "prog";
char pass[] = "Alvorlig5And";

int pos = 0; 
byte mac[] = { 0xDE, 0xAD, 0xBE, 0x3F, 0xFE, 0xED };   //physical mac address
byte ip[] = { 192,168,56,100 };                        //ip in lan (that's what you need to use in your browser. ("192.168.56.100")
WiFiServer server(80);                             //Server port    
WiFiClient wifi_client;

// API functionality
char serverAddress[] = "0357-93-176-82-58.ngrok.io";  // server address
int port = 80;
HttpClient httpClient = HttpClient(wifi_client, serverAddress, port);

bool apiConnectionError = false;

String readString;            //HTTP request read

int ERROR_PIN = 10;

void setup() {
 // Open serial monitor and wait for port to open
  Serial.begin(9600);
   while (!Serial) {
    ; // wait for serial port to connect. Needed for Leonardo only
  }

  pinMode(13, OUTPUT);
  pinMode(ERROR_PIN, OUTPUT);

  connect_to_wifi(ssid, pass);

  // print your WiFi shield's IP address:
  IPAddress ip = WiFi.localIP();
  Serial.print("IP Address: ");
  Serial.println(ip);
  server.begin();  

  DEVICE_IP = ip;


  apiConnectionError = false;
  Serial.println("making API request");
  //httpClient.get("/devices/IAmOnline");

  String contentType = "application/json; charset=UTF-8";
  String postData = "DeviceTypeId=1&UniqueDeviceIdentifier=1234&IPAddress=10.108.138.79";

  httpClient.get("/devices/IAmOnline?" + postData);
  //httpClient.post("/devices/IAmOnline", contentType, postData);

  // read the status code and body of the response
  int statusCode = httpClient.responseStatusCode();

  Serial.print("Status code: ");
  Serial.println(statusCode);
  
  if(statusCode == 200 || statusCode == 203){  
    String response = httpClient.responseBody();
    
    Serial.print("Response: ");
    Serial.println(response);
  }
  else{
    apiConnectionError = true;
  }
  
}


void loop() {
  // Check for client request
  
  if(apiConnectionError == true){

    if(digitalRead(ERROR_PIN) == HIGH){
      digitalWrite(ERROR_PIN, LOW);
    }
    else{
      digitalWrite(ERROR_PIN, HIGH);
    }
    delay(1000);
    return;
  }
  
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
           if (readString.indexOf("?function1") >0){
               Serial.println("Function 1 has been called.");
               digitalWrite(13, HIGH);
               client.println("HTTP/1.1 204 OK"); 
           }
           else if (readString.indexOf("?function2") >0){
               Serial.println("Function 2 has been called.");
               digitalWrite(13, LOW);
               client.println("HTTP/1.1 204 OK"); 
           }
           else if(readString == ""){
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
              client.println("HTTP/1.1 400 Bad Request");     
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
}
