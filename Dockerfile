# --- Etapa 1: Build da Aplica��o ---
# Usando a imagem do SDK do .NET 9 para compilar o projeto.
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia o arquivo .csproj e restaura as depend�ncias primeiro.
# Isso aproveita o cache do Docker, acelerando builds futuros.
COPY *.csproj .
RUN dotnet restore

# Copia o restante do c�digo-fonte e publica a aplica��o.
COPY . .
RUN dotnet publish -c Release -o /app --no-restore

# --- Etapa 2: Imagem Final de Runtime ---
# Usando a imagem do ASP.NET Runtime, que � menor e mais segura.
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Define a vari�vel de ambiente para que o Kestrel escute na porta 8080,
# correspondendo ao que a instru��o EXPOSE documenta.
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Cria um usu�rio n�o-root para executar a aplica��o por seguran�a.
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

# Copia os arquivos publicados da etapa de build.
COPY --from=build /app ./

# Define o ponto de entrada para iniciar a aplica��o.
ENTRYPOINT ["dotnet", "ReportWebMvc.dll"]
