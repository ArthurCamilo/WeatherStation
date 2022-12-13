//**** CLIENTE Estacao Metereologica ********

//placa "NodeMCU 1.0(ESP-12E Module)"
#include <DHT.h>
#define DHTPIN 4 // pino do sensor D2 - GPIO4 - SDA (não pode ser outro)
#define DHTTYPE DHT11 //tipo de sensor
DHT dht(DHTPIN, DHTTYPE);
#include <ESP8266WiFi.h> // no gerenciador de bibliotecas: "ESP8266WIFI by Ivan Grokhotkov"
#include <PubSubClient.h> //Biblioteca.zip disponível em https://github.com/knolleary/pubsubclient
WiFiClient wifiClient;

//configuração WiFi
const char* SSID = "Galaxy M12AAE4"; //NOME da Rede LAN
const char* PASSWORD = "eiyi8569"; //SENHA da rede LAN

//Configuração do Cliente MQTT
const char* BROKER_MQTT = "192.168.141.24";
int BROKER_PORT = 1883; // Porta do Broker MQTT
#define ID_MQTT "Est_Met" //Nome do Cliente, que deve ser único no Broker.
#define TOPIC_PUB_TEMP "Est_Met/Temperatura" //Topico para publicar Temperatura
#define TOPIC_PUB_UMID "Est_Met/Umidade" //Topico para publicar Umidade
PubSubClient MQTT(wifiClient); // Instancia o Cliente MQTT passando o objeto espClient
unsigned long tempoAnterior = 0;

void setup(){
  Serial.begin(9600);
  dht.begin();
  conectaWiFi(); //conecta ao wifi
  MQTT.setServer(BROKER_MQTT, BROKER_PORT); //configura o cliente
  MQTT.setCallback(mqtt_callback); //ativa a função para receber postagens de tópicos assinados
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
  Serial.println(WiFi.localIP()); //Retorna o endereço IP atribuído após a conexão.
}

void conectaMQTT(){
  while(!MQTT.connected()) {
    Serial.print("Conectando ao Broker MQTT: ");
    Serial.println(BROKER_MQTT);
    if(MQTT.connect(ID_MQTT)) {
      Serial.println("Conectado ao Broker com sucesso!");
    }
    else {
      Serial.println("Falha ao conectar, nova tentativa em 10s");
      delay(10000);
    }
  }
}

void publicaDados(){
  if(millis() - tempoAnterior > 5000){//leitura a cada 5s
    tempoAnterior = millis();
    char MsgUmidadeMQTT[10];
    char MsgTemperaturaMQTT[10];
    float temperatura = dht.readTemperature(); //Le a temperatura
    float umidade = dht.readHumidity(); //Le a Humidade
    sprintf(MsgTemperaturaMQTT,"%.1f",temperatura);
    MQTT.publish(TOPIC_PUB_TEMP,MsgTemperaturaMQTT); //publica no tópico "Est_Met/Temperatura"
    sprintf(MsgUmidadeMQTT,"%.1f",umidade);
    MQTT.publish(TOPIC_PUB_UMID,MsgUmidadeMQTT); //publica no tópico "Est_Met/Umidade"
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
}

void loop() {
  if(!MQTT.connected())conectaMQTT();//caso não haja conexão, refaz
  if (WiFi.status() != WL_CONNECTED) conectaWiFi(); //caso não haja conexão, refaz
  publicaDados();
  MQTT.loop();//keep alive do protocolo
}