# RabbitMQ + C# + WPF + MVVM – Learning Repository

### About This Repository  
This is my personal learning playground while studying **RabbitMQ** message broker together with **C#**, **WPF (Windows Presentation Foundation)**, and the **MVVM (Model-View-ViewModel)** architectural pattern.

Everything here is for **educational purposes only** – expect messy code, half-finished experiments, commented-out sections, and a lot of “Aha!” moments as I figure things out.

### Topics I’m Exploring
- Setting up RabbitMQ locally (Windows + Erlang)
- Basic messaging patterns
- Publish/Subscribe scenarios in WPF desktop apps
- Implementing clean MVVM in WPF
- Async/await with RabbitMQ consumers
- Using `RabbitMQ.Client` NuGet package
- Sending and receiving complex objects (JSON serialization)
- Background message consumers that update the WPF UI safely

### Tech Stack
- .NET 8 (some folders still on .NET 6/7)
- C#
- WPF (.NET)
- RabbitMQ.Client (NuGet)
- CommunityToolkit.Mvvm (for MVVM)
- JSON serialization (both Newtonsoft.Json and System.Text.Json)
- RabbitMQ server running on **Ubuntu 24.04 LTS** inside **VirtualBox**

### Local RabbitMQ Setup (for me and anyone cloning this)
```bash
# Using Docker (recommended)
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management

### My RabbitMQ Setup (VirtualBox + Ubuntu)
Instead of running RabbitMQ directly on Windows or Docker, I’m hosting it inside an **Ubuntu 24.04 LTS** virtual machine using **VirtualBox**

**Quick steps I followed (for future me or anyone curious):**
```bash
# Inside Ubuntu VM
sudo apt update && sudo apt upgrade -y
sudo apt install curl gnupg apt-transport-https -y

# Install Erlang (required by RabbitMQ)
wget https://packages.erlang-solutions.com/erlang-solutions_2.0_all.deb
sudo dpkg -i erlang-solutions_2.0_all.deb
sudo apt update
sudo apt install erlang

# Install RabbitMQ
curl -s https://packagecloud.io/install/repositories/rabbitmq/rabbitmq-server/script.deb.sh | sudo bash
sudo apt install rabbitmq-server -y

# Enable management plugin
sudo rabbitmq-plugins enable rabbitmq_management

# Start the server
sudo systemctl start rabbitmq-server
sudo systemctl enable rabbitmq-server
