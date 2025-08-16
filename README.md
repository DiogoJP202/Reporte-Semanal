# Gerador de Report HTML com IA

## 📖 Sobre o Projeto

O **Gerador de Report HTML** é uma aplicação web desenvolvida em ASP.NET Core MVC que automatiza a criação de reports de notícias em formato HTML. A ferramenta utiliza a **API do Gemini (Google AI)** para processar e separar inteligentemente os parágrafos do texto, e oferece uma interface de usuário dinâmica para montar o report com múltiplas notícias.

Originalmente um programa de console, este projeto foi migrado para uma solução web completa, containerizada com Docker, para facilitar o uso e a implantação.

## ✨ Funcionalidades Principais

  * **Formulário Dinâmico:** Adicione ou remova campos de notícia de forma interativa.
  * **Processamento com IA:** Utiliza a API do Gemini para estruturar o texto e identificar a localização de imagens de forma inteligente.
  * **Geração de HTML:** Cria um arquivo HTML completo e estilizado, pronto para ser usado em e-mails ou páginas web.
  * **Interface Moderna:** UI limpa e responsiva com animações, construída com Bootstrap 5.
  * **Visualizador de Código:** Exibe o código HTML gerado em um modal com *syntax highlighting* e um botão para cópia rápida.
  * **Pronto para Deploy:** O projeto inclui um `Dockerfile` otimizado para facilitar a implantação em qualquer ambiente que suporte contêineres.

## 🛠️ Tecnologias Utilizadas

  * **Backend:** C\# com .NET 9 e ASP.NET Core MVC
  * **Frontend:** HTML5, CSS3, JavaScript, Bootstrap 5
  * **Inteligência Artificial:** Google Gemini API
  * **Bibliotecas:**
      * `Newtonsoft.Json` para manipulação de JSON.
      * `Prism.js` para formatação e realce de sintaxe do código.
  * **Containerização:** Docker

## 🚀 Como Executar o Projeto

Siga os passos abaixo para executar a aplicação em seu ambiente local ou via Docker.

### Pré-requisitos

  * [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
  * [Docker Desktop](https://www.docker.com/products/docker-desktop/) (Opcional, para execução via contêiner)
  * Uma chave de API do [Google AI Studio (Gemini)](https://aistudio.google.com/app/apikey)

### 1\. Configuração Local

1.  **Clone o repositório:**
    ```bash
    git clone https://github.com/seu-usuario/seu-repositorio.git
    cd seu-repositorio
    ```
2.  **Configure a Chave da API:**
      * Abra o arquivo `appsettings.json`.
      * Insira sua chave da API do Gemini no campo `ApiKey`:
        ```json
        "Gemini": {
          "ApiKey": "SUA_CHAVE_DA_API_AQUI"
        }
        ```
3.  **Restaure as dependências e execute:**
    ```bash
    dotnet restore
    dotnet run
    ```
4.  Abra seu navegador e acesse a URL indicada no terminal (geralmente `https://localhost:7...` ou `http://localhost:5...`).

### 2\. Execução com Docker

1.  **Clone o repositório e configure a chave da API** conforme os passos 1 e 2 da configuração local.
2.  **Construa a imagem Docker:**
    Na raiz do projeto, execute o comando:
    ```bash
    docker build -t gerador-report .
    ```
3.  **Execute o contêiner:**
    ```bash
    docker run -p 8080:8080 gerador-report
    ```
4.  Abra seu navegador e acesse `http://localhost:8080`.

## 📂 Estrutura do Projeto

```
/
├── Controllers/      # Controladores MVC (lógica de requisição/resposta)
│   └── Reporte/
│       └── FormatadorNoticiaController.cs
├── Models/           # Modelos de dados e ViewModels
│   └── Reporte/
│       └── ReportDataViewModel.cs
├── Views/            # Arquivos .cshtml (interface do usuário)
│   ├── Reporte/
│   │   └── Index.cshtml
│   └── Shared/
├── Services/         # Lógica de negócio (ex: chamada à API Gemini)
│   └── Reporte/
│       └── FormatadorNoticiaService.cs
├── wwwroot/          # Arquivos estáticos (CSS, JS, imagens)
├── Dockerfile        # Define a imagem Docker da aplicação
├── .dockerignore     # Especifica arquivos a serem ignorados pelo Docker
└── README.md         # Esta documentação
```
