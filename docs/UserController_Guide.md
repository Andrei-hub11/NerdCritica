# Endpoints:

### Registra Usuário

- URL: /api/v1/register
- Método HTTP: POST
- Cabeçalho de Autenticação: Não é necessário autenticação.
- Corpo da Solicitação (JSON):

```
{
"UserName": "Nome do Usuário",
"Email": "email@example.com",
"Password": "senha",
"Role": "Nome da Função"
}
```

- Resposta de Sucesso (200 OK):

```
{
"token": "Token de Autenticação"
"User": {
        "Id": "b642caad-d46d-4e31-bcd9-97828dcf5d74",
        "UserName": "nathalia1117",
        "Email": "nathalia@1227gmail.com",
        "ProfileImagePath": null,
        "Tickets": []
    },
    "Role": [
        "User"
    ]
}
```

- Resposta de Erro (400 Bad Request):

```
{
"Message": "Os campos não foram corretamente preenchidos",
"Errors": ["Erro 1", "Erro 2", ...]
}
```

- Resposta de Erro (500 Internal Server Error):

```
{
"Message": "Ocorreu um erro durante o registro.",
"Error": "Mensagem de erro específica"
}
```

### Login de Usuário

- URL: /api/v1/login
- Método HTTP: POST
- Cabeçalho de Autenticação: Não é necessário autenticação.
- Corpo da Solicitação (JSON):

```
{
  "Email": "email@example.com",
  "Password": "senha"
}
```

- Resposta de Sucesso (200 OK):

```
{
"token": "Token de Autenticação"
  "User": {
        "Id": "b642caad-d46d-4e31-bcd9-97828dcf5d74",
        "UserName": "nathalia1117",
        "Email": "nathalia@1227gmail.com",
        "ProfileImagePath": null,
    },
}
```

- Resposta de Erro (401 Unauthorized):

```
{
"Message": "Credenciais inválidas."
}
```

- Resposta de Erro (500 Internal Server Error):

```
{
"Message": "Ocorreu um erro durante o login.",
"Error": "Mensagem de erro específica"
}
```

### Recupera Dados do Usuário

- Endpoint: /api/v1/get-me
- Método HTTP: GET
- Cabeçalho: Exige o token JWT de usuário do tipo 'Bearer' que contenha a função 'Admin', 'Moderator' ou 'User' ao ser decodificado.
- Resposta de Sucesso (200 OK):

```
{
    "User": {
        "Id": "b642caad-d46d-4e31-bcd9-97828dcf5d74",
        "UserName": "nathalia1117",
        "Email": "nathalia@1227gmail.com",
        "ProfileImagePath": null,
        "Tickets": []
    },
    "Role": [
        "User"
    ]
}
```

- Resposta de Erro (401 Unauthorized):

```
{
"Message": "Credenciais inválidas."
}
```

- Resposta de Erro (500 Internal Server Error):

```
{
"Message": "Ocorreu um erro ao tentar recuperar o usuário.",
"Error": "Mensagem de erro específica"
}
```
