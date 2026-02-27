# Copilot Instructions — autovenda-backends-dotnet

## Quem sou eu

Senior Software Engineer com 15+ anos de .NET/C#.
Experiência real com: WCF, Web API 4.x, Entity Framework, Redis,
SQL Server, PostgreSQL, FinTech, sistemas de pagamento, Flutter,
Angular, React. Prêmio de performance Avivatec 2024.

Estou **reativando e modernizando** meu conhecimento .NET —
não aprendendo do zero. Conheço bem os padrões antigos.
Quero entender o que mudou, por que mudou, e qual é o
equivalente moderno.

Este repositório é também portfólio e preparação para
entrevistas técnicas sênior.

---

## REGRA PRINCIPAL — NUNCA IGNORE ESTA SEÇÃO

**Nunca tome decisões técnicas ou arquiteturais por mim.**

Quando surgir um ponto de decisão, sempre:
1. PAUSE — descreva a decisão que precisa ser tomada
2. Apresente o que a maioria dos times faz em produção hoje
3. Liste 2 a 4 opções reais com prós e contras breves
4. Diga qual você recomendaria e por quê
5. ESPERE minha decisão antes de escrever qualquer código

Isso vale para tudo — desde "qual pacote NuGet usar" até
"como estruturar este endpoint". Eu aprendo decidindo,
não recebendo soluções prontas.

---

## Como quero que você explique as coisas

- Compare sempre o .NET moderno com o equivalente antigo
  Exemplo: "Minimal API substitui X do Web API 4.x porque..."
- Sinalize quando algo é frequentemente cobrado em
  entrevistas sênior
- Prefira código a parágrafos — leio código mais rápido
- Se eu escrever algo que funcionava antes mas tem
  uma forma melhor hoje, mostre os dois e explique a evolução
- Use sempre C# 12+ e .NET 8/9

---

## Este projeto — AutoVenda

MVP de vendas de veículos para um vendedor autônomo no Brasil.
Contexto completo: docs/architecture/ — leia todos os arquivos.

Linguagem ubíqua do domínio — use sempre em código e comentários:
Proposta, Piso, Pacotão, Martelinho, BuyerProfile,
PerfilDoComprador, Anuncio, Catalogo, LeadRecord

6 Bounded Contexts:
Catalogue | Negotiation | Identity & Profiling |
Media Protection | Security & Trust | Backoffice

---

## Estrutura deste repositório

```
src/AutoVenda.MinimalApi       → REST Minimal API (.NET 9)
src/AutoVenda.WebApi           → Controllers Web API (futuro)
src/AutoVenda.gRPC             → gRPC + Protobuf (futuro)
src/AutoVenda.SignalR          → Tempo real (futuro)
src/AutoVenda.GraphQL          → HotChocolate GraphQL (futuro)
shared/AutoVenda.Domain        → Domínio compartilhado
shared/AutoVenda.Infrastructure → Infra compartilhada
```

---

## Regras absolutas

1. Este repositório é **exclusivamente C#/.NET**
   Nunca sugira Node.js, JavaScript ou TypeScript aqui
2. Apresente decisões antes de implementar — sempre
3. Nunca assuma escolha de pacote — apresente opções
4. Código de estudo deve ter qualidade de produção:
   error handling correto, sem magic strings, sem secrets no código
5. Nullable reference types sempre habilitado
6. XML doc comments em todo método público de classes de domínio