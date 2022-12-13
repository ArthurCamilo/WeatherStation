# WeatherStation

## Hardware

- Duas placas NodeMCU ESP8266;
- Um sensor de temperatura e umidade DHT11;
- Um display LCD i2c

Para a gravação dos códigos fonte nas placas, foi utilizada a IDE => Arduino IDE

### Funcionamento das placas

A primeira placa NodeMCU atua como cliente publisher num Broker MQTT, ela é responsável pela coleta dos dados do sensor de temperatura e umidade DHT11. Com o código no arquivo `hardware/Est_Met.ino`
A segunda placa NodeMCU atua como cliente subscriber num Broker MQTT, ela é responsável por mostrar os valores medidos no display LCD. Com o código no arquivo  `hardware/Supervisor`
Obs: Modificar nos dois arquivos `.ino` o IP e outras configurações do Broker.

## Aplicação

### Tecnologias utilizadas

- Mosquitto MQTT Server (Com execução na máquina Windows ou Ubuntu)
  - Liberar config
  ```
    listener 1883 0.0.0.0
    allow_anonymous true
  ```
- Banco de dados SQLite (Modificar no código o caminho do arquivo em `Data Source=C:\\Users\\Arthur Camilo\\Documents\\UDESC\\Redes\\database.sqlite`)
- Foi utilizado Visual Studio para a execução do código C#

Obs: Configurar caminho do banco de dados e IP Address do broker
