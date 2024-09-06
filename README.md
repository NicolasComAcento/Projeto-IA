PT-BR

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

Caso tenha uma GPU e deseje usar CUDA use o seguinte comando em vez do anterior:
pip install torch torchvision torchaudio --index-url https://download.pytorch.org/whl/cu118

2. Instalando o ML-Agents
   
Instale a biblioteca mlagents e os pacotes necessários para treinar e executar modelos no Unity.
pip install mlagents==0.29.0

IMPORTANTE: certifique-se que todo processo anterior foi feito com as versões corretas, caso contrário o mlagents vai dar problema.

4. Executando um Treinamento de Teste
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

Se tudo deu certo essa tela deve aparecer, então é só der Play no editor do Unity e começar seus testes.

4. Monitorando o Treinamento
   
Para uma melhor visualização dos resultados, como mostrado no artigo usamo o TensorBoard, para instala-lo use o seguinte comando

pip install tensorboard e tensorboard --logdir="results" para visualização, os resultados do treino vão automaticamente para a bata results.

5. Usando modelo já treinado no Unity
   
Quando o treinamento terminar, o modelo treinado será salvo na pasta results. Esse modelo pode ser usado diretamente no Unity para fazer inferências.
Importe o modelo gerado no Unity:
Acesse results > escolha a pasta do treino desejado e selecione o arquivo .onnx gerado.
Aplique o modelo ao agente em sua cena para que ele utilize o comportamento treinado.

Qualquer dúvida entrar em contato com Nicolaspnovaes@gmail.com
Documentação oficial do ML-Agents -> https://github.com/Unity-Technologies/ml-agents

_______________________________________________________________________________________________________________________________________________________________________________________________________
ENGLISH

[Presentation(only pt-br)](/apresentacao) 

[Research Report(only pt-br)](/relatorio)

[Scripts](/Assets/Scripts/)

Basic Guide to Setting Up Unity ML-Agents
Configuration Guide - Unity ML-Agents
This guide outlines the necessary steps to configure the environment and run tests using Unity ML-Agents with PyTorch support.

Requirements
Unity (version 2022.3.26f1 for this project)
Python 3.7 or higher
pip installed

1. Installing PyTorch
ML-Agents uses PyTorch as the backend. Install it according to your operating system and GPU configuration (optional).
CPU
pip install torch torchvision torchaudio
If you have a GPU and wish to use CUDA, use the following command instead of the previous one:
pip install torch torchvision torchaudio --index-url https://download.pytorch.org/whl/cu118

2. Installing ML-Agents
Install the mlagents library and the necessary packages to train and run models in Unity.
pip install mlagents==0.29.0
IMPORTANT: Ensure that all previous steps were completed with the correct versions; otherwise, mlagents may encounter issues.

3. Running a Test Training
After opening Unity with the project scene, follow these steps in the CMD:

Navigate to the ML-Agents project directory:
cd C:\Users\nicol\ml-agents  # This is where your project was downloaded; just copy the path

Activate the virtual environment:
venv\Scripts\activate  # On Windows

Ensure that the config.yaml file is in the Unity project folder:
cd Project  # Replace 'Project' with the actual path

Start the training:
mlagents-learn config.yaml --run-id=TestNameHere
If you want to run with SAC, use this command to start the training. If you prefer PPO, you can omit config.yaml.

![image](https://github.com/user-attachments/assets/89484d4e-0d25-49bb-84b4-532bfd5cc1ca)

If everything went well, this screen should appear. Then, just click Play in the Unity editor and start your tests.

4. Monitoring the Training
For better visualization of the results, as shown in the article, we use TensorBoard. To install it, use the following command:
pip install tensorboard

Then, run:
tensorboard --logdir="results"
This will allow you to view the results, which will automatically be saved in the results directory.

5. Using a Pre-Trained Model in Unity
When training is complete, the trained model will be saved in the results folder. This model can be directly used in Unity for inference.

Import the generated model into Unity: Access results > select the desired training folder and choose the generated .onnx file.
Apply the model to the agent in your scene so that it utilizes the trained behavior.
For any questions, contact: Nicolaspnovaes@gmail.com
Official ML-Agents Documentation -> https://github.com/Unity-Technologies/ml-agents







