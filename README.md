Versão Unity 2022.3.26f1 

# Relatório 

[Apresentação](/apresentacao)
[Relatório](/relatorio)
[Códigos](/Assets/Scripts/)

Vídeo de um dos primeiros testes usando PPO: https://youtu.be/zAxB5Y-3joY
Vídeo mostrando SAC treinado: https://youtu.be/u-IZWOWod_M

Guia básico de como configurar o Unity ML-Agents

Guia de Configuração - Unity ML-Agents
Este guia descreve os passos necessários para configurar o ambiente e rodar testes utilizando o Unity ML-Agents com suporte para PyTorch.

Requisitos
Unity (versão 2022.3.26f1 para esse projeto)
Python 3.7 ou superior
pip instalado

1. Instalando o PyTorch
O ML-Agents utiliza o PyTorch como backend. Instale-o de acordo com seu sistema operacional e configuração de GPU (opcional).
CPU
pip install torch torchvision torchaudio
Caso tenha uma GPU e deseje usar KUDA use o seguinte comando em vez do anterior:
pip install torch torchvision torchaudio --index-url https://download.pytorch.org/whl/cu118

2. Instalando o ML-Agents
Instale a biblioteca mlagents e os pacotes necessários para treinar e executar modelos no Unity.
pip install mlagents==0.29.0
IMPORTANTE certifique-se que todo processo anterior foi feito com as versões corretas, caso contrário o mlagents vai dar problema.

3. Executando um Treinamento de Teste
Depois de abrir o Unity na cena do projeto siga os seguintes passos no CMD

Navegar para o diretório do projeto ml-agents
cd C:\Users\nicol\ml-agents # Vai ser onde o seu projeto foi baixado basta copiar o caminho

Ativar o ambiente virtual
venv\Scripts\activate  # No Windows

Certifique-se de que o arquivo config.yaml está na pasta do projeto Unity
cd Project  # Coloca a pasta no lugar de Project, só copiar o caminho

Iniciar o treinamento
mlagents-learn config.yaml --run-id=NomeDoTesteAqui # Se quiser rodar com o SAC isso deve ser usado para inciar o treinamento, caso queria com o PPO basta tirar o config.yaml

![image](https://github.com/user-attachments/assets/89484d4e-0d25-49bb-84b4-532bfd5cc1ca)

Se tudo deu certo essa tela deve aparecer, então é só der Play no editor do Unity e começar seus testes. Qualquer dúvida entrar em contato com Nicolaspnovaes@gmail.com
