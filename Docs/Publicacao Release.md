# Sistemas e Ferramentas Utilizados na Implantação

## Objetivo

Para a implantação do sistema **Inventory Masters** foram utilizadas ferramentas que garantem a compilação, empacotamento, instalação e execução da aplicação em computadores clientes. A escolha de cada ferramenta levou em consideração a compatibilidade com as tecnologias utilizadas no projeto, facilidade de manutenção e confiabilidade durante o processo de instalação.

---

# Visual Studio 2022

O **Visual Studio 2022** foi utilizado como ambiente oficial de desenvolvimento e compilação do sistema.

Por meio dele foi possível desenvolver, depurar, testar e gerar a versão final da aplicação em modo **Release**, garantindo que o software fosse compilado sem dependências do ambiente de desenvolvimento.

A ferramenta também permitiu o gerenciamento dos pacotes **NuGet**, referências do projeto, configuração da arquitetura **x64** e validação do funcionamento do sistema antes da criação do instalador.

### Justificativa da escolha

O Visual Studio foi escolhido por ser o ambiente oficial para desenvolvimento de aplicações **.NET** e **WPF**, oferecendo total compatibilidade com o projeto e integração com todas as bibliotecas utilizadas.

---

# .NET Framework 4.8

A aplicação desktop responsável pela comunicação com o **Kinect** foi desenvolvida utilizando o **.NET Framework 4.8**.

Essa plataforma oferece suporte às bibliotecas necessárias para comunicação com o **Kinect SDK**, **Entity Framework 6** e **SQLite**.

### Justificativa da escolha

O **Kinect SDK 1.8** possui suporte nativo ao **.NET Framework**, tornando essa versão a alternativa mais estável e compatível para o funcionamento da aplicação desktop.

---

# ASP.NET Core .NET 8

A aplicação web foi desenvolvida utilizando **ASP.NET Core .NET 8**.

Essa aplicação é responsável pelo gerenciamento dos usuários, autenticação, dashboard, armazenamento em nuvem, comunicação em tempo real e disponibilização das APIs utilizadas pelo aplicativo desktop.

### Justificativa da escolha

O **.NET 8** oferece alto desempenho, estabilidade, suporte de longo prazo (**LTS**) e excelente integração com serviços em nuvem, sendo adequado para aplicações corporativas.

---

# Inno Setup

O **Inno Setup** foi utilizado para criar o instalador da aplicação desktop.

A ferramenta possibilitou gerar um único arquivo executável responsável por copiar todos os arquivos necessários, criar atalhos, registrar o sistema no Windows e disponibilizar um desinstalador.

Também foi configurada a criação automática das pastas utilizadas para armazenamento de dados e logs da aplicação.

### Justificativa da escolha

O **Inno Setup** foi escolhido por ser gratuito, amplamente utilizado no mercado e permitir a criação de instaladores profissionais com baixo nível de complexidade.

Além disso, atende aos requisitos da atividade proposta e oferece recursos suficientes para distribuir a aplicação desktop.

---

# SQLite

O **SQLite** foi utilizado como banco de dados local da aplicação desktop.

Seu objetivo é armazenar temporariamente medições, histórico de ocupação e informações necessárias para o funcionamento da aplicação, mesmo quando não houver comunicação imediata com o servidor.

### Justificativa da escolha

Foi escolhido por ser um banco leve, de fácil distribuição, que não necessita de instalação de servidor e possui excelente desempenho para aplicações desktop.

---

# Firebase

O **Firebase** foi utilizado como banco de dados principal da aplicação web.

Nele são armazenadas as informações corporativas, usuários, empresas, configurações e demais dados sincronizados entre os clientes.

### Justificativa da escolha

Foi escolhido devido à alta disponibilidade, escalabilidade, facilidade de integração com aplicações .NET e baixa necessidade de administração de infraestrutura.

---

# Kinect SDK 1.8

O **Kinect SDK 1.8** foi utilizado para permitir a comunicação entre o sistema e o sensor **Kinect Xbox 360**.

Através dele é possível acessar os sensores de profundidade e imagem utilizados no cálculo volumétrico do estoque.

### Justificativa da escolha

Foi escolhido por ser a biblioteca oficial da Microsoft para o **Kinect v1**, oferecendo estabilidade e suporte às funcionalidades necessárias para o projeto.

---

# Git e GitHub

O controle de versões do projeto foi realizado utilizando **Git** e **GitHub**.

Essas ferramentas permitiram controlar alterações no código, manter histórico das versões e facilitar o trabalho colaborativo entre os integrantes da equipe.

### Justificativa da escolha

Foram escolhidos por serem padrões de mercado para versionamento de software, proporcionando segurança, rastreabilidade e colaboração durante o desenvolvimento.

---

# Conclusão

A combinação das ferramentas utilizadas permitiu desenvolver, testar, implantar e distribuir o sistema **Inventory Masters** de forma organizada e segura.

O **Visual Studio 2022** foi empregado para o desenvolvimento e compilação do sistema, enquanto o **ASP.NET Core .NET 8** e o **.NET Framework 4.8** forneceram a base tecnológica para as aplicações web e desktop. O **Firebase** foi utilizado como banco de dados em nuvem e o **SQLite** como banco de dados local da aplicação desktop, garantindo armazenamento e sincronização das informações.

A integração com o **Kinect SDK 1.8** possibilitou a captura dos dados necessários para o cálculo volumétrico do estoque, enquanto o **Inno Setup** foi responsável pela criação do instalador da aplicação, simplificando sua distribuição e instalação nos computadores clientes.

Por fim, o uso do **Git** e do **GitHub** proporcionou controle de versões, colaboração entre os integrantes da equipe e rastreabilidade das alterações realizadas durante todo o desenvolvimento do projeto.
