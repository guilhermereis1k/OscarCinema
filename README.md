# OscarCinema

O **OscarCinema** é uma aplicação de gerenciamento de cinema que permite aos usuários consultar filmes, horários de sessões, selecionar assentos e realizar pedidos de ingressos. O projeto possui duas camadas principais:

1. **Backend** – originalmente desenvolvido em **Spring Boot (Java 17)** mas em processo de migração, responsável por:
   - Gerenciar entidades como `User`, `Movie`, `Room`, `Seat` e `Order`.
   - Lidar com persistência no banco de dados (PostgreSQL).
   - Implementar regras de negócio e validações.
   - Expor endpoints REST para consumo do frontend.

2. **Frontend** – desenvolvido em **React**, com funcionalidades de:
   - Exibição de filmes e sessões.
   - Seleção de assentos.
   - Finalização de pedidos.
   - Consumo das APIs do backend.

O backend está sendo migrado para **.NET Core**, buscando padronizar o stack de desenvolvimento.

---

## Estrutura do Projeto (Planejada)

**Backend (.NET Core):**
- `Domain` – entidades e regras de negócio.
- `Application` ou `Services` – camada intermediária entre controllers e domain.
- `Infrastructure` – acesso a banco de dados (Entity Framework Core) e configuração de persistência.
- `API` – controllers e endpoints REST.

**Frontend (React):**
- Componentes para exibição de filmes, sessões e assentos.
- Context API para gerenciamento de estado do pedido.
- Comunicação com backend via Axios.

---

## Migração do Spring para .NET

O projeto ainda está em fase inicial de migração. Até o momento, foram criadas as **entidades (models) em C#**, correspondentes às classes existentes no backend Spring.  
O restante da migração, incluindo serviços, controllers e integração com banco de dados, ainda está em andamento.

---

## Observações

- O objetivo da migração é manter a lógica original do Spring, mas otimizar a arquitetura do projeto, usando clean architecture e conceitos de DDD, e adaptar às convenções e padrões do .NET Core.  
- Após a migração completa do backend, será possível integrar o frontend React já existente sem grandes alterações.
