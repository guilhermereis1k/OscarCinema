# OscarCinema

O **OscarCinema** é uma aplicação completa para gestão de cinemas, cobrindo desde catálogo de filmes até sessões, assentos, venda de ingressos e administração geral.  
O sistema foi inicialmente desenvolvido em **Spring** e reimplementado em **.NET**, aplicando práticas de engenharia de software, DDD e Clean Architecture para garantir organização, desacoplamento e fácil evolução.

---

## Modelagem do Domínio

### Entidades Principais
- **Movie** — Título, imagem, duração, classificação indicativa e gênero.  
- **Room** — Sala com múltiplos assentos reserváveis.  
- **Seat** — Assento (ex.: A12), com tipo que altera preço.  
- **Session** — Relaciona filme, sala e horário, considerando trailers, filme e limpeza. Possui *ExhibitionType*.  
- **Ticket** — Ingresso vinculado a um usuário e sessão, exigindo assentos e tipo (inteira/meia).  
- **TicketSeat** — Liga Ticket ↔ Seat, definindo o tipo por assento.  
- **User** — Administrador, Funcionário ou Cliente; cada um com permissões específicas.

### Entidades de Precificação
- **ExhibitionType** — IMAX, FullHD, etc., cada tipo com preço.  
- **SeatType** — VIP, Normal, Reclinável, etc., também com preço próprio.

> Se quiser, posso transformar tudo isso em tabelas técnicas.

---

## Regras de Negócio

- Cálculo preciso da duração real da sessão (trailers + filme + limpeza).  
- Prevenção de reserva duplicada de assentos.  
- Preço final = `SeatType + ExhibitionType + Tipo do ticket (inteira/meia)`.  
- **Ticket** funciona como *agregado*, contendo **TicketSeat**.

---

## Arquitetura

O projeto segue uma variação pragmática de **Clean Architecture**:

/Domain → Entidades, agregados e regras de negócio
/Application → DTOs, interfaces, serviços e casos de uso
/Infrastructure → Persistência, repositórios e migrations
/API → Controllers, endpoints, validações e DI
/CrossCutting → Configuração e registro de dependências


### Motivações
- Separação verdadeira entre **negócio** e **infraestrutura**  
- Melhor testabilidade  
- Baixo acoplamento  
- Expansão simples para novos fluxos e precificações

---

## Fluxo Principal de Compra

1. Seleção do filme  
2. Escolha da sessão  
3. Exibição dos assentos disponíveis  
4. Seleção dos assentos  
5. Definição do tipo de ticket por assento  
6. Geração de **Ticket** e **TicketSeat**  
7. Cálculo final do preço

---

## Documentação da API

Swagger disponível em:

/swagger


### Principais rotas

/movies
/rooms
/sessions
/seats
/tickets
/users


---

## Como Rodar o Projeto

### Requisitos
- .NET 8  
- MySQL  
- Docker (opcional)

---

## Rodando com Docker Compose

O repositório inclui um `docker-compose.yml` que sobe:

- MySQL  
- API do OscarCinema  

### Subir

docker compose up -d


### Derrubar

docker compose down


A API ficará disponível em:

http://localhost:8080


---

## Rodando Manualmente

### Atualizar banco

dotnet ef database update


### Executar API

dotnet run


---

## Decisões de Design

- **TicketSeat** evita acoplamento direto entre Ticket e Seat.  
- **ExhibitionType** e **SeatType** isolados para evolução da precificação.  
- **CrossCutting** criado para centralizar injeção e configuração.  
- Regras de negócio mantidas no **Domínio**.  
- Fluxos críticos implementados na camada **Application**.

---

## Pontos de Extensibilidade

- Produtos adicionais no fluxo de compra  
- Sistema de fidelidade  
- Preço dinâmico por horário   
