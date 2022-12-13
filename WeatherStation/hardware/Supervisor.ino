//****** CLIENTE SUPERVISOR *********
//placa "NodeMCU 1.0(ESP-12E Module)"
#include <Wire.h>

#include "SSD1306Wire.h"
//Pinos do NodeMCU
// SDA => D1
// SCL => D2
// Inicializa o display Oled
SSD1306Wire  display(0x3c, D1, D2);


#include <ESP8266WiFi.h> // no gerenciador de bibliotecas: "ESP8266WIFI by Ivan Grokhotkov"
#include <PubSubClient.h> //Biblioteca.zip disponível em https://github.com/knolleary/pubsubclient

//configuração WiFi
const char* SSID = "Galaxy M12AAE4"; //NOME da Rede LAN
const char* PASSWORD = "eiyi8569"; //SENHA da rede LAN
WiFiClient wifiClient;

//Configuração do Cliente MQTT
const char* BROKER_MQTT = "192.168.141.24"; 
int BROKER_PORT = 1883; // Porta do Broker MQTT
#define ID_MQTT "SUPERVISOR" //Nome do Cliente, que deve ser único no Broker.

#define TOPIC_SUB_TEMP "Est_Met/Temperatura" //Topico para assinatura da temperatura
#define TOPIC_SUB_UMID "Est_Met/Umidade" //Topico para Assinatura da umidade
PubSubClient MQTT(wifiClient); // Instancia o Cliente MQTT passando o objeto espClient

void setup(){
  Serial.begin(9600);
  conectaWiFi();//conecta ao wifi
  MQTT.setServer(BROKER_MQTT, BROKER_PORT);//configura o cliente
  MQTT.setCallback(mqtt_callback);//ativa a função para receber postagens de tópicos assinados

  display.init();
  display.flipScreenVertically();
  //Apaga o display
  display.clear();
  display.setTextAlignment(TEXT_ALIGN_CENTER);
  //Seleciona a fonte
  display.setFont(ArialMT_Plain_16);
  display.drawString(63, 10, "Temp.[oC]: ");
  //display.setCursor(0,1);
  display.drawString(63, 26, "Umid.[%]: ");
  display.display();
}


void conectaWiFi(){
  Serial.println("");
  Serial.print("Conectando a Rede: ");
  WiFi.begin(SSID, PASSWORD); // Conecta na rede WI-FI
  while (WiFi.status() != WL_CONNECTED){delay(100);Serial.print(".");}
  Serial.println();
  Serial.print("Conectado com sucesso, na rede: ");
  Serial.print(SSID);
  Serial.print(" IP obtido: ");
  Serial.println(WiFi.localIP());
}

void conectaMQTT(){
  while(!MQTT.connected()) {
    Serial.print("Conectando ao Broker MQTT: ");
    Serial.println(BROKER_MQTT);
    if(MQTT.connect(ID_MQTT)) {
      Serial.println("Conectado ao Broker com sucesso!");
      MQTT.subscribe(TOPIC_SUB_TEMP); //assina o tópico "Cliente/A/B0"
      MQTT.subscribe(TOPIC_SUB_UMID); //assina o tópico "Cliente/A/B6"
    }
    else{
      Serial.println("Falha ao conectar, nova tentativa em 10s");
      delay(10000);
    }
  }
}

//função que recebe mensagens dos tópicos assinados
void mqtt_callback(char* topic, byte* payload, unsigned int length){
  String topico = topic;
  String mensagem;//obtem a string do payload recebido
  for(int i = 0; i < length; i++){
    char c = (char)payload[i];
    mensagem += c;
  }
  if(topico.equals(TOPIC_SUB_TEMP)){//lê o topico "Est_Met/Temperatura"
    Serial.print("Temperatura: ");
    Serial.println(mensagem);
    display.clear();
    display.drawString(63, 10, "Temp.[oC]: " + String(mensagem));
    display.display();
  }


  if(topico.equals(TOPIC_SUB_UMID)){//lê o topico "Est_Met/Umidade"
    Serial.print("Umidade: ");
    Serial.println(mensagem);
    display.drawString(63, 26, "Umid.[%]: " + String(mensagem));
    display.display();
  }
}

void loop() {
  if(!MQTT.connected())conectaMQTT();//caso não haja conexão, refaz
  if (WiFi.status() != WL_CONNECTED) conectaWiFi(); //caso não haja conexão, refaz
  MQTT.loop();//keep alive do protocolo
}