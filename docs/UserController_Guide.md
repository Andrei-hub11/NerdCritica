# Endpoints:

### Registra Usuário

- URL: /api/v1/account/register
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
     "Message": "Registro bem sucedido. Seja bem-vindo",
     "User": {
        "Id": "b642caad-d46d-4e31-bcd9-97828dcf5d74",
        "IdentityUserId": "2f4b23a1-0c10-4551-86bc-05e4447f6023",
        "UserName": "nathalia1117",
        "Email": "nathalia@1227gmail.com",
        "ProfileImagePath": "",
    },
    "Token": "Token de Autenticação"
    "Role": "User"
}
```

- Resposta de Erro (400 Bad Request):

```
{
"Message": "Os campos não foram corretamente preenchidos",
"Errors": ["Erro 1", "Erro 2", ...]
}
```

### Login de Usuário

- URL: /api/v1/account/login
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
  "User": {
        "Id": "b642caad-d46d-4e31-bcd9-97828dcf5d74",
        "UserName": "nathalia1117",
        "Email": "nathalia@1227gmail.com",
        "ProfileImagePath": null,
    },
    "Token": "Token de Autenticação",
    "Role": "User"
}
```

- Resposta de Erro (401 Unauthorized):

```
{
"Message": "Credenciais inválidas."
}
```

### Recupera Dados do Usuário

- Endpoint: /api/v1/account/get-me
- Método HTTP: GET
- Cabeçalho: Exige o token JWT de usuário do tipo 'Bearer' que contenha a função 'Admin', 'Moderator' ou 'User' ao ser decodificado.
- Resposta de Sucesso (200 OK):

```
{
    "User": {
        "Id": "b642caad-d46d-4e31-bcd9-97828dcf5d74",
        "IdentityUserId": "2f4b23a1-0c10-4551-86bc-05e4447f6023",
        "UserName": "nathalia1117",
        "Email": "nathalia@1227gmail.com",
        "ProfileImagePath": "",
    },
    "Token": "Token de Autenticação",
    "Role": "User"
}
```

- Resposta de Erro (401 Unauthorized):

```
{
"Message": "Credenciais inválidas."
}
```

### Adiciona um Filme à Lista de Favoritos do Usuário

- Endpoint: /api/v1/account/favorite-movie
- Método HTTP: POST
- Descrição: Este endpoint permite aos usuários adicionar um filme à sua lista de favoritos.
- Cabeçalhos: Exige o token JWT de usuário do tipo 'Bearer' que contenha a função 'Admin', 'Moderator' ou 'User' ao ser decodificado.
- Corpo da Solicitação (JSON):

```
{
"IdentityUserId": "b642caad-d46d-4e31-bcd9-97828dcf5d74",
"MoviePostId": "5f1f8d1a-1cb2-4b2d-b0e2-45e6633037ef"
}
```

- Resposta de Sucesso (200 OK):

```
true
```

- Resposta de Erro (401 Unauthorized):

```
{
"Message": "Credenciais inválidas."
}
```

### Atualiza Usuário

- Endpoint: /api/v1/account/update/{identityUserId}
- Método HTTP: PUT
- Cabeçalhos: Exige o token JWT de usuário do tipo 'Bearer' que contenha a função 'Admin', 'Moderator' ou 'User' ao ser decodificado.
- Corpo da Solicitação (JSON):

```
{
    "Username": "nathalia_updated",
    "Email": "nathalia_updated@gmail.com",
    "ProfileImage": "base64_encoded_image_data"
}
```

- Resposta de Sucesso (200 OK):

```
{
  "Message": "A atualização foi bem-sucedida.",
  "User": {
      "Id": "b642caad-d46d-4e31-bcd9-97828dcf5d74",
      "IdentityUserId": "2f4b23a1-0c10-4551-86bc-05e4447f6023",
      "UserName": "nathalia_updated",
      "Email": "nathalia_updated@gmail.com",
      "ProfileImagePath": "caminho_da_imagem_do_perfil"
  }
}
```

- Resposta de Erro (401 Unauthorized):

```
{
"Message": "Credenciais inválidas."
}
```
