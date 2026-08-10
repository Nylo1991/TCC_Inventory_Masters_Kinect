# Sistemas e Ferramentas Utilizados na Implantação

## Objetivo

Para a implantação do sistema **Inventory Masters** foram utilizadas ferramentas responsáveis pela compilação, publicação, empacotamento, instalação e execução das aplicações desenvolvidas.

Durante o processo foram gerados dois tipos de instaladores para a aplicação desktop (EXE e MSI), permitindo diferentes estratégias de distribuição do sistema.

A escolha de cada tecnologia levou em consideração a compatibilidade com a arquitetura da solução, facilidade de manutenção, estabilidade e integração com o ambiente Windows.

---

## Visual Studio 2022

O Visual Studio 2022 foi utilizado como ambiente oficial de desenvolvimento da solução.

Por meio dele foi possível desenvolver, depurar, testar, compilar e publicar tanto a aplicação desktop (WPF) quanto a aplicação Web (ASP.NET Core MVC).

Também foi utilizado para gerenciamento de dependências, referências do projeto, pacotes NuGet e configuração dos perfis de publicação.

### Justificativa da escolha

O Visual Studio foi escolhido por ser o ambiente oficial de desenvolvimento da plataforma .NET, oferecendo integração completa com todas as tecnologias utilizadas no projeto.

---

## .NET Framework 4.8

A aplicação desktop responsável pela comunicação com o Kinect foi desenvolvida utilizando o .NET Framework 4.8.

Essa plataforma oferece suporte nativo ao Kinect SDK 1.8, Entity Framework 6, SQLite e demais bibliotecas utilizadas durante o desenvolvimento.

### Justificativa da escolha

Foi escolhida por oferecer total compatibilidade com o Kinect SDK 1.8 e estabilidade para aplicações desktop.

---

## ASP.NET Core .NET 8

A aplicação Web foi desenvolvida utilizando ASP.NET Core .NET 8.

Essa aplicação é responsável por:

- autenticação;
- gerenciamento de usuários;
- gerenciamento das empresas;
- dashboard;
- armazenamento em nuvem;
- APIs;
- comunicação em tempo real através do SignalR.

Após a compilação foi realizada a publicação da aplicação em modo **Release**, gerando uma pasta contendo todos os arquivos necessários para execução fora do ambiente de desenvolvimento.

### Justificativa da escolha

Foi escolhido devido ao seu alto desempenho, suporte LTS (Long Term Support), segurança e integração com serviços em nuvem.

---

## Publicação da Aplicação Web (MVC)

Após a conclusão do desenvolvimento da aplicação Web, foi realizada a publicação (Publish) do projeto ASP.NET Core MVC utilizando o Visual Studio 2022.

Foi adotado o perfil de publicação em pasta (Folder Publish), gerando uma versão independente da aplicação pronta para execução fora do ambiente de desenvolvimento.

A publicação foi configurada utilizando:

- Configuration: **Release**
- Target Framework: **.NET 8**
- Deployment Mode: **Framework-dependent**
- Target Runtime: **Portable**

Ao final da publicação foi gerada uma pasta contendo todos os arquivos necessários para execução da aplicação Web.

Entre os principais arquivos gerados destacam-se:

- MVC_InventoryMasters.exe
- MVC_InventoryMasters.dll
- MVC_InventoryMasters.deps.json
- MVC_InventoryMasters.runtimeconfig.json
- appsettings.json
- web.config
- bibliotecas (.dll)
- pasta wwwroot
- arquivos estáticos da aplicação

A publicação foi armazenada na pasta:

```text
C:\PublicacaoInventoryMastersMVC
```

Essa estrutura permite executar a aplicação diretamente através do executável ou realizar posteriormente sua implantação em um servidor IIS ou outro ambiente compatível com ASP.NET Core.

### Justificativa da escolha

A publicação em modo **Release** foi adotada por gerar uma versão otimizada da aplicação, removendo informações de depuração e reunindo todos os arquivos necessários para execução em ambiente de produção.

A utilização do modo **Framework-dependent** reduz o tamanho da publicação, aproveitando o runtime .NET já instalado no servidor, facilitando futuras atualizações do sistema.

---

## Executável da Aplicação Web

Durante o processo de publicação foi gerado automaticamente o executável da aplicação Web:

```text
MVC_InventoryMasters.exe
```

Esse executável inicializa o servidor Web da aplicação utilizando o Kestrel, permitindo que o sistema seja executado fora do Visual Studio.

Durante os testes realizados, a aplicação foi iniciada com sucesso, disponibilizando o sistema através do endereço:

```text
http://localhost:5000
```

Essa validação confirmou que a publicação da aplicação foi realizada corretamente e que todos os arquivos necessários para sua execução foram gerados com sucesso.

  -------

## Publicação da Aplicação Desktop

A aplicação desktop foi compilada em modo **Release x64**.

Durante a publicação foram incluídas automaticamente todas as bibliotecas necessárias para funcionamento da aplicação, incluindo:

- Kinect SDK;
- SQLite;
- Entity Framework;
- SignalR Client;
- bibliotecas auxiliares.

Também foi realizada a configuração das referências do projeto para que todas as dependências fossem copiadas automaticamente para a pasta de publicação.

---

## Inno Setup

O Inno Setup foi utilizado para criação do instalador principal da aplicação desktop.

Foi gerado um instalador no formato:

```
InventoryMastersKinect-Setup-1.0.0.exe
```

Esse instalador é responsável por:

- copiar todos os arquivos da aplicação;
- instalar o sistema;
- criar atalhos;
- registrar o desinstalador;
- permitir atualização de versões;
- remover versões anteriores quando necessário.

### Justificativa da escolha

Foi escolhido por ser uma ferramenta gratuita, consolidada no mercado e extremamente flexível para criação de instaladores profissionais.

---

## WiX Toolset

Também foi utilizado o WiX Toolset para geração de um pacote de instalação no padrão Windows Installer.

Foi gerado o arquivo:

```
InventoryMastersKinect-1.0.0.msi
```

Esse pacote realiza:

- instalação da aplicação;
- registro junto ao Windows Installer;
- integração com o gerenciamento de aplicativos do Windows;
- suporte à instalação em ambientes corporativos.

### Justificativa da escolha

Foi escolhido para demonstrar a utilização do padrão MSI (Microsoft Installer), amplamente empregado em ambientes corporativos para distribuição e gerenciamento de aplicações.

---

## SQLite

O SQLite foi utilizado como banco de dados local da aplicação desktop.

Seu objetivo é armazenar temporariamente informações necessárias ao funcionamento da aplicação, permitindo persistência local dos dados.

Durante a implantação, o banco de dados passou a ser criado automaticamente na pasta:

```
C:\ProgramData\Inventory Masters\Dados
```

evitando problemas de permissão de escrita na pasta da aplicação.

### Justificativa da escolha

Foi escolhido por ser leve, rápido, portátil e não exigir instalação de servidor.

---

## Firebase 

O Firebase foi utilizado como banco de dados principal da aplicação Web.

Nele são armazenadas informações como:

- usuários;
- empresas;
- configurações;
- medições;
- histórico;
- demais dados sincronizados entre os clientes.

### Justificativa da escolha

Foi escolhido devido à alta disponibilidade, escalabilidade e facilidade de integração com aplicações .NET.

---

## Kinect SDK 1.8

O Kinect SDK foi utilizado para comunicação entre o sistema e o sensor Kinect Xbox 360.

Através dele são obtidos os dados de profundidade utilizados no cálculo volumétrico do estoque.

### Justificativa da escolha

Foi escolhido por ser o SDK oficial da Microsoft para o Kinect v1 e oferecer todas as funcionalidades necessárias ao projeto.

---

## Git e GitHub

Durante todo o desenvolvimento foi utilizado Git para controle de versões e GitHub para armazenamento remoto do código-fonte.

Essas ferramentas permitiram manter o histórico das alterações, facilitar o trabalho colaborativo e garantir rastreabilidade durante o desenvolvimento.

### Justificativa da escolha

Foram escolhidos por serem padrões de mercado para versionamento de software.

---

## Arquivos Gerados na Implantação

Ao final do processo de implantação foram gerados os seguintes artefatos:

### Aplicação Web

- Publicação da aplicação ASP.NET Core MVC
- Pasta completa de publicação para servidor

### Aplicação Desktop

- Publicação Release x64

### Instaladores

- **InventoryMastersKinect-Setup-1.0.0.exe** (Inno Setup)
- **InventoryMastersKinect-1.0.0.msi** (WiX Toolset)

---

## Conclusão

A implantação do sistema Inventory Masters foi realizada utilizando ferramentas consolidadas do ecossistema Microsoft.

Foram geradas publicações independentes para a aplicação Web e para a aplicação Desktop, além de dois formatos distintos de instaladores (EXE e MSI), permitindo diferentes estratégias de distribuição do sistema.

Essa abordagem garante facilidade de instalação, manutenção, atualização e futura implantação em ambientes corporativos, atendendo aos requisitos definidos para o desenvolvimento do projeto de conclusão de curso.

---

