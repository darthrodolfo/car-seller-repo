# autovenda-backends-dotnet

Repositório de estudo e portfólio — backends .NET do projeto AutoVenda.

Cada pasta em `src/` é uma API independente explorando um tipo diferente
de backend em C#/.NET 9. Todas compartilham o mesmo domínio (`shared/`)
e rodam na mesma infraestrutura local (Docker Compose).

---

## APIs de estudo

| API | Tipo | Status | Porta |
|-----|------|--------|-------|
| `AutoVenda.MinimalApi` | REST Minimal API | 🟢 Ativo | 5001 |
| `AutoVenda.WebApi` | Controllers Web API | 🔜 Em breve | 5002 |
| `AutoVenda.gRPC` | gRPC + Protobuf | 🔜 Em breve | 5003 |
| `AutoVenda.SignalR` | Real-time | 🔜 Em breve | 5004 |
| `AutoVenda.GraphQL` | HotChocolate | 🔜 Em breve | 5005 |

---

## Stack

- **Runtime:** .NET 9 / C# 12
- **Cache:** Redis 7
- **Infra local:** Docker Compose
- **Infra cloud:** Kubernetes + Kustomize
- **Domínio:** DDD Strategic Design (ver `docs/architecture/`)

---

## Início rápido

```bash
# 1. Clone e entre na pasta
git clone https://github.com/SEU_USUARIO/autovenda-backends-dotnet
cd autovenda-backends-dotnet

# 2. Sobe tudo
make up

# 3. Testa
curl http://localhost:5001/health
# → { "status": "healthy" }

# Swagger (só em Development)
open http://localhost:5001/swagger
```

### Com Redis Commander (UI do Redis)
```bash
make up-tools
# Redis Commander → http://localhost:8081
```

---

## Estrutura

```
autovenda-backends-dotnet/
├── .github/
│   └── copilot-instructions.md   ← contexto para o GitHub Copilot
├── .vscode/
├── src/
│   └── AutoVenda.MinimalApi/     ← primeira API de estudo
├── shared/
│   ├── AutoVenda.Domain/         ← entidades e domínio compartilhado
│   └── AutoVenda.Infrastructure/ ← Redis, Postgres (quando entrar)
├── k8s/
│   ├── base/                     ← manifests Kubernetes base
│   └── overlays/
│       ├── local/                ← para minikube/kind
│       └── production/           ← para cloud
├── docs/
│   └── architecture/             ← ADRs e documento de MVP
├── docker-compose.yml
├── Makefile
├── .env.example
├── CHANGELOG.md
└── CONTRIBUTING.md
```

---

## Comandos úteis

```bash
make help          # lista todos os comandos
make up            # sobe Redis + Minimal API
make down          # para tudo
make clean         # para tudo e apaga volumes
make logs-api      # tail dos logs da API
make shell-api     # shell no container da API
make shell-redis   # redis-cli no container
make k8s-local     # aplica no cluster local (minikube/kind)
make k8s-status    # lista pods no namespace autovenda-dotnet
```

---

## Decisões de arquitetura

Todas as decisões técnicas relevantes estão documentadas em ADRs:
`docs/architecture/ADR-000X-titulo.md`

Antes de qualquer mudança significativa — leia os ADRs existentes.
Se a mudança for significativa — escreva um novo ADR.

---

## Contexto do projeto

AutoVenda é um MVP de vendas de veículos para um único vendedor
autônomo no Brasil. Contexto completo: `docs/architecture/`.
