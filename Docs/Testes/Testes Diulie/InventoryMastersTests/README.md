# Suíte de testes das ViewModels

Componente escolhido: camada `ViewModel` da aplicação WPF Inventory Masters Kinect.

## Escopo

- `BaseViewModel`: alteração de propriedades e evento `PropertyChanged`.
- `KinectLoginViewModel`: estado inicial, troca de abas, validação do token, solicitação de token, respostas de sucesso, recusa e exceção.
- `MainViewModel`: validação da sessão, estados iniciais, comandos e notificações.
- `MainViewModel.Espaco`: campos obrigatórios, valores inválidos, limites de 1% e 100%, calibração obrigatória e salvamento válido.
- `MainViewModel.Volume`: conversão de unidades, ocupação, espaço livre, alerta, limite de 100% e medição sem Kinect.
- `MainViewModel.Historico`: carregamento, atualização e falha do repositório.
- `MainViewModel.Interface`: bloqueio por inatividade, desbloqueio, identidade da sessão e solicitação de novo token.
- `MainViewModel.Kinect`: desligamento seguro sem sensor conectado.

Os serviços de autenticação e repositório são substituídos por objetos falsos. Assim, os testes não dependem do MVC publicado, de e-mail, de banco real nem de um Kinect conectado.

## Resultado atual

- Total: 61 testes.
- Aprovados: 61.
- Reprovados: 0.
- Ambiente: xUnit, .NET Framework 4.8, plataforma x64.

Durante o desenvolvimento, um teste falhou por esperar separador decimal dependente da cultura. A expectativa foi corrigida para representar o contrato real da propriedade no cenário sem calibração.

## Fora do escopo unitário

Captura RGB/depth, calibração física e leitura volumétrica real exigem o Kinect conectado. Esses comportamentos devem ser cobertos por testes de integração ou testes manuais com o hardware.

## Execução

Abra a solução no Visual Studio, selecione a plataforma `x64`, compile o projeto de testes e execute pelo Test Explorer.
