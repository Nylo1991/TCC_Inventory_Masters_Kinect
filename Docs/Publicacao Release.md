# Sistemas e Ferramentas Utilizados na Implantação

## Objetivo

Para a implantação do sistema **Inventory Masters** foram utilizadas ferramentas que garantem a compilação, empacotamento, instalação e execução da aplicação em computadores clientes. A escolha de cada tecnologia levou em consideração a compatibilidade com os requisitos do projeto, a facilidade de manutenção e a confiabilidade durante todo o processo de implantação.

---

## Visual Studio 2022

O **Visual Studio 2022** foi utilizado como o ambiente de desenvolvimento integrado (IDE) oficial para a construção e compilação do sistema.

Por meio dele foi possível desenvolver, depurar, testar e gerar a versão final da aplicação em modo **Release**, garantindo que o software fosse compilado sem dependências do ambiente de desenvolvimento.

A ferramenta também viabilizou o gerenciamento dos pacotes **NuGet**, o controle de referências, a configuração da arquitetura **x64** e a validação do funcionamento do sistema antes da geração do instalador.

### Justificativa da escolha

O Visual Studio foi escolhido por ser o ambiente padrão da Microsoft para o desenvolvimento de aplicações **.NET** e **WPF**, oferecendo total compatibilidade e integração nativa com todas as bibliotecas utilizadas no projeto.

---

## .NET Framework 4.8

A aplicação desktop responsável pela interface e pela comunicação direta com o sensor **Kinect** foi desenvolvida utilizando o **.NET Framework 4.8**.

Essa plataforma fornece o suporte essencial às bibliotecas do **Kinect SDK**, do **Entity Framework 6** e do **SQLite**.

### Justificativa da escolha

O **Kinect SDK 1.8** possui dependência e suporte nativo voltados ao ecossistema do .NET Framework, tornando esta versão a alternativa mais estável e compatível para a execução segura da aplicação desktop de captura física.

---

## ASP.NET Core 8

A aplicação web e os serviços de retaguarda foram desenvolvidos utilizando o **ASP.NET Core 8**.

Essa plataforma gerencia o ecossistema de usuários, autenticação, painéis (*dashboards*), persistência em nuvem e a comunicação em tempo real via **SignalR** para a integração com o aplicativo desktop.

### Justificativa da escolha

O **.NET 8** oferece alto desempenho, estabilidade, suporte de longo prazo (**LTS**) e excelente integração nativa com serviços em nuvem, destacando-se como a melhor escolha para arquiteturas corporativas modernas.

---

## Inno Setup

O **Inno Setup** foi a ferramenta selecionada para a criação do instalador da aplicação desktop.

A tecnologia permitiu consolidar a distribuição em um único arquivo executável responsável por copiar os arquivos necessários, criar atalhos, registrar componentes no Windows e estruturar um desinstalador limpo.

Também automatizou a criação das estruturas de diretórios locais exigidas para o armazenamento de dados e logs da aplicação.

### Justificativa da escolha

O **Inno Setup** destaca-se por ser uma ferramenta gratuita, amplamente validada pelo mercado e capaz de gerar instaladores profissionais e customizáveis com baixa complexidade operacional.

---

## SQLite

O **SQLite** foi adotado como o banco de dados relacional local da aplicação desktop.

Seu principal objetivo é armazenar de forma embarcada as medições, o histórico operacional e os estados locais, garantindo a continuidade das operações mesmo em cenários de instabilidade ou ausência de conexão com a rede.

### Justificativa da escolha

Foi escolhido por ser um banco de dados leve, autossuficiente (sem necessidade de instalação de serviços de servidor) e de alta performance para cenários de persistência local (*edge computing*).

---

## Firebase

O **Firebase** foi utilizado como a infraestrutura de nuvem inicial para suporte à aplicação web e sincronização de dados.

Nele foram estruturadas as informações de autenticação, perfis corporativos e os dados compartilhados entre os clientes.

### Justificativa da escolha

Sua adoção deu-se pela alta disponibilidade, facilidade de integração rápida com o ecossistema .NET e baixa sobrecarga na administração inicial de infraestrutura.

---

## Kinect SDK 1.8

O **Kinect SDK 1.8** foi a biblioteca de interface utilizada para estabelecer a comunicação direta entre o software e o sensor **Kinect Xbox 360**.

Através dele, o sistema obtém os fluxos de profundidade e mapeamento espacial indispensáveis para o cálculo volumétrico do estoque.

### Justificativa da escolha

Trata-se do kit de desenvolvimento oficial da Microsoft para a primeira geração do sensor, garantindo estabilidade de drivers e acesso direto às matrizes de profundidade requeridas pelo projeto.

---

## Git e GitHub

O controle de versões e a gestão de configuração do código-fonte foram conduzidos utilizando o **Git** em conjunto com o **GitHub**.

Essas ferramentas viabilizaram o versionamento incremental, o rastreamento histórico de alterações e o fluxo de trabalho colaborativo entre os membros da equipe.

### Justificativa da escolha

São ferramentas consolidadas como padrão de mercado, proporcionando segurança, auditoria e facilidade na integração contínua durante o ciclo de vida do desenvolvimento.

---

## Conclusão

A seleção harmônica das tecnologias empregadas permitiu projetar, desenvolver, testar e implantar o **Inventory Masters** de maneira organizada, escalável e segura.

Enquanto o **Visual Studio 2022** direcionou a engenharia do software, o **ASP.NET Core 8** e o **.NET Framework 4.8** sustentaram as camadas web e desktop, respectivamente. O uso combinado do **SQLite** (local) e do **Firebase** (nuvem inicial) garantiu a flexibilidade de dados, complementada pela integração direta com o **Kinect SDK 1.8** para a captura física precisa. 

Por fim, o empacotamento via **Inno Setup** simplificou a distribuição nos nós clientes, e o ecossistema **Git/GitHub** assegurou a integridade e a colaboração técnica ao longo de todo o projeto.
