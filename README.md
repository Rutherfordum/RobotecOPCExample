# RobotecOPCExample
Общайтесь с помощью унифицированной архитектуры OPC и Visual Studio. С помощью этой библиотеки ваше приложение может просматривать, читать, записывать и подписываться на оперативные данные, публикуемые серверами OPC UA в вашей сети.

Поддерживает .NET Core, универсальную платформу Windows (UWP), Windows Presentation Framework (WPF) и приложения Xamarin.

# Настройка проекта
1. Установите пакет Workstation.UaClientиз Nuget , чтобы получить последнюю версию для вашего проекта hmi.
2. Добавьте в проект наш скрипт OpcClient.cs

# Подключение к серверу OPC
```
var client = new OpcClient(IP:PORT", "UserName", "Password");
```
Подпишитесь на события подключения
```
client.connected += Client_connected;
client.disconnected += Client_disconnected;
client.Connect();
```

# Пример чтения ноды 
```
var pos_x = client.ReadNode("ns=5;s=MotionDeviceSystem.ProcessData.R1.System.$config.pos_x");
```
# Пример записи ноды 
```
client.WriteNode("ns=5;s=MotionDeviceSystem.ProcessData.R1.System.$config.time", 150);
```

