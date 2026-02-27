# AutoVenda MVP — Documento de Arquitetura e Design

> **Versão 1.0 — 2026**
> DDD Strategy Phase | UX Artifacts | BDD Scenarios | Instrumentation Strategy

---

## Índice

- [PARTE 1 — Domain-Driven Design (DDD)](#parte-1--domain-driven-design-ddd--strategy-phase)
  - [1.1 Executive Summary & Problem Statement](#11-executive-summary--problem-statement)
  - [1.2 Ubiquitous Language Glossary](#12-ubiquitous-language-glossary)
  - [1.3 Bounded Contexts Map](#13-bounded-contexts-map)
  - [1.4 Core Domain vs Supporting Domains](#14-core-domain-vs-supporting-domains)
  - [1.5 Domain Events Catalogue](#15-domain-events-catalogue)
  - [1.6 Aggregate Roots & Entities](#16-aggregate-roots--entities)
  - [1.7 Context Map — Strategic Relationships](#17-context-map--strategic-relationships)
- [PARTE 2 — UX Artifacts](#parte-2--ux-artifacts)
  - [2.1 User Personas](#21-user-personas)
  - [2.2 Information Architecture](#22-information-architecture)
  - [2.3 User Journey Maps](#23-user-journey-maps)
  - [2.4 Wireframe Annotations](#24-wireframe-annotations)
  - [2.5 UX Design Principles & Patterns](#25-ux-design-principles--patterns)
- [PARTE 3 — BDD Scenarios](#parte-3--behaviour-driven-development-bdd)
- [PARTE 4 — Instrumentation & Metrics Strategy](#parte-4--instrumentation--metrics-strategy)

---

## PARTE 1 — Domain-Driven Design (DDD) — Strategy Phase

### 1.1 Executive Summary & Problem Statement

AutoVenda é uma plataforma mobile-first de vendas de veículos servindo um único vendedor autônomo no mercado brasileiro. O desafio central é construir uma ferramenta de vendas de alta conversão que permaneça resiliente em conectividade 4G instável, proteja os ativos de mídia do vendedor contra uso fraudulento, e converta navegadores anônimos em leads qualificados através de um fluxo gamificado e de construção de confiança.

**Business Goals**

- Eliminar fricção desde a primeira visita até o primeiro contato (Zero-Friction Onboarding)
- Qualificar a intenção do comprador antes de coletar dados pessoais
- Proteger a marca e os ativos de mídia do vendedor em nível de infraestrutura
- Operar completamente offline com sync inteligente ao restaurar conectividade
- Segmentar leads por categoria de comprador para personalizar o follow-up

**Key Constraints**

| Constraint | Context | Design Response |
|------------|---------|-----------------|
| Connectivity | 4G/Wi-Fi instável | Offline-First + sync Connectivity-Aware |
| Data Cost | Dados móveis pré-pagos no Brasil são caros | 4G: JSON + capa; HD sob demanda |
| Trust | Listagens falsas na OLX/FB usando mídia roubada | Watermark server-side + overlay de vídeo |
| Conversion | Alto abandono em login walls | Validação de identidade pós-intenção apenas |

---

### 1.2 Ubiquitous Language Glossary

Estes termos são compartilhados entre design, engenharia e negócio. Todo artefato deste documento os utiliza de forma consistente.

| Termo | Definição |
|-------|-----------|
| **Catálogo** | O conjunto completo e versionado de listagens de veículos do vendedor |
| **Pacotão JSON** | O payload de sync diferencial que substitui o catálogo local quando detectada divergência de hash |
| **Hash de Versão** | Fingerprint curto (SHA-256 truncado) do catálogo que clientes comparam com o servidor para detectar obsolescência |
| **Anúncio** | Uma única entrada de veículo no catálogo, contendo mídia, preço, referência FIPE e pitch de áudio do vendedor |
| **Martelinho** | O fluxo gamificado de oferta, nomeado após o martelo de leilão. Ponto de entrada: botão "Fazer Oferta" |
| **Piso de Negociação** | Valor mínimo aceitável configurado pelo vendedor. Propostas abaixo deste valor são rejeitadas no cliente antes do envio |
| **Lead** | Qualquer usuário que acionou a validação de identidade pós-intenção, independente de ter concluído uma negociação |
| **Proposta** | Uma oferta de preço estruturada e não vinculante enviada através do fluxo Martelinho |
| **Interesse** | Uma solicitação de contato suave ("Tenho Interesse") que não inclui oferta de preço |
| **Perfil do Comprador** | Categoria selecionada na validação de identidade: PF, Vendedor Autônomo, Lojista ou Investidor |
| **Blur-Gate** | Padrão de UX que obscurece mídia HD com overlay de desfoque e prompt de conectividade quando a rede está indisponível |
| **Indicação** | Referral in-app onde um usuário validado compartilha o app com até 3 contatos de terceiros (limite lifetime) |
| **X9 Report** | Flag de fraude acionada pela comunidade, onde um usuário reporta uma listagem falsa suspeita em plataformas externas |
| **Confete** | Animação de sucesso exibida após envio de uma Proposta, reforçando gamificação e feedback positivo |
| **Pitch de Áudio** | Nota de voz gravada pelo vendedor anexada a um Anúncio, protegida com injeção de ruído contra clonagem de voz por IA |
| **Backoffice** | Painel administrativo do vendedor para gestão de catálogo, upload de mídia, revisão de leads e configuração de sync |

---

### 1.3 Bounded Contexts Map

| Bounded Context | Core Responsibilities | Key Entities |
|-----------------|----------------------|--------------|
| **Catalogue** | Versionamento do catálogo, geração do Pacotão JSON, gestão de hash, freshness de dados offline | Listing, Catalogue, HashVersion, MediaAsset |
| **Negotiation** | Fluxo Martelinho, enforcement do Piso, lifecycle da Proposta, trigger do Confete | Proposta, Piso, OfferStatus, PillSuggestion |
| **Identity & Profiling** | Validação WhatsApp/SMS pós-intenção, categorização do BuyerProfile, qualificação de lead | BuyerIdentity, PerfilDoComprador, LeadRecord |
| **Media Protection** | Injeção de watermark, overlay de vídeo FFmpeg, injeção de ruído em áudio, conversão WebP | MediaAsset, WatermarkJob, AudioObfuscationJob |
| **Security & Trust** | Limite anti-spam de referral, ingestão de X9 report, roteamento de flag de fraude | ReferralRecord, X9Report, SpamGuard |
| **Backoffice** | Dashboard de leads, pipeline de upload de mídia, trigger de versionamento, config de taxas de financiamento | AdminUser, LeadView, UploadJob, FinancingRate |

---

### 1.4 Core Domain vs Supporting Domains

| Domain | Classification | Rationale |
|--------|---------------|-----------|
| Negotiation (Martelinho) | **CORE** | O fluxo de oferta gamificado com enforcement do Piso é o principal diferencial competitivo do produto |
| Catalogue (Offline-First) | **CORE** | Sync connectivity-aware com hash versioning é diferencial técnico chave para o mercado brasileiro |
| Media Protection | **SUPPORTING** | Watermarking e obfuscação de áudio protegem ativos do vendedor — forte sinal de confiança |
| Identity & Profiling | **SUPPORTING** | Validação pós-intenção e segmentação de compradores habilitam roteamento personalizado de leads |
| Security & Trust | **SUPPORTING** | Anti-spam e X9 protegem a integridade do marketplace mas não são capacidades únicas |
| Backoffice | **GENERIC** | CRUD administrativo padrão; poderia usar CMS off-the-shelf com customização em iterações futuras |

---

### 1.5 Domain Events Catalogue

| Domain Event | Description & Payload |
|-------------|----------------------|
| `CatalogueHashChecked` | Disparado no app open ou restore do background. Payload: localHash, serverHash |
| `CatalogueSyncRequired` | Disparado quando localHash != serverHash. Aciona download do Pacotão |
| `CatalogueSyncCompleted` | Disparado quando o Pacotão JSON foi persistido no DB local. Payload: listingCount, mediaMode (HD/Cover) |
| `ListingOpened` | Disparado quando o usuário abre um Anúncio. Aciona carregamento HD sob demanda em 4G |
| `NegotiationInitiated` | Disparado quando o usuário toca "Negociar Agora". Payload: listingId, channelType (Interesse/Proposta) |
| `IdentityValidationTriggered` | Disparado pós-intenção quando o usuário precisa verificar via WhatsApp/SMS |
| `BuyerProfileSelected` | Disparado quando o usuário seleciona uma categoria de Perfil do Comprador |
| `IdentityVerified` | Disparado quando a verificação OTP/WhatsApp é bem-sucedida. Desbloqueia envio de Proposta |
| `PropostaSubmitted` | Disparado quando a oferta passa pelo Piso e é enviada ao backend. Aciona animação Confete |
| `PropostaRejectedByPiso` | Disparado quando a oferta está abaixo do mínimo do vendedor. Nenhuma chamada de rede é feita |
| `InterestLeadCreated` | Disparado quando o formulário de contato "Tenho Interesse" é enviado |
| `PropostaStatusUpdated` | Disparado quando o vendedor responde via Backoffice (Contactado / Pendente) |
| `MediaBlurGateTriggered` | Disparado quando o usuário tenta ver mídia HD sem conectividade |
| `VideoPlaybackRequested` | Disparado quando o usuário clica em play em 4G. Payload: fileSize, listingId |
| `X9ReportSubmitted` | Disparado quando o usuário envia um report de fraude. Payload: platform, listingId, reporterId |
| `ReferralLimitExceeded` | Disparado quando um usuário tenta indicar mais de 3 contatos (lifetime) |
| `BackofficeLeadViewed` | Disparado quando o vendedor abre um registro de lead. Aciona PropostaStatusUpdated |
| `MediaUploadCompleted` | Disparado quando o vendedor faz upload de mídia. Aciona pipeline de watermark/noise-injection |

---

### 1.6 Aggregate Roots & Entities

#### Aggregate: Listing (Root do Catalogue Context)

| Type | Classification | Key Attributes |
|------|---------------|----------------|
| `Listing` | Aggregate Root | listingId, title, price, pisoDeNegociacao, fipeReference (optional), status |
| `MediaAsset` | Entity | assetId, type (image/video/audio), url, thumbnailUrl, watermarked, sizeBytes |
| `HashVersion` | Value Object | sha256Hash, generatedAt, mediaMode |
| `PillSuggestion` | Value Object | percentage (-5/-10/-15), computedValue |

#### Aggregate: Proposta (Root do Negotiation Context)

| Type | Classification | Key Attributes |
|------|---------------|----------------|
| `Proposta` | Aggregate Root | propostaId, listingId, buyerIdentityId, offeredValue, status (Enviada/Contatada) |
| `PisoGuard` | Domain Service | validate(offeredValue, piso): bool — enforced client-side antes de qualquer chamada de rede |
| `OfferStatus` | Value Object | enum: Enviada / ContatoRealizado |

#### Aggregate: BuyerIdentity (Root do Identity Context)

| Type | Classification | Key Attributes |
|------|---------------|----------------|
| `BuyerIdentity` | Aggregate Root | identityId, phone, verificationChannel (WhatsApp/SMS), verifiedAt |
| `PerfilDoComprador` | Entity | category: PF / VendedorAutonomo / Lojista / Investidor |
| `LeadRecord` | Entity | leadId, identityId, type (Interesse/Proposta), listingId, createdAt |
| `ReferralRecord` | Value Object | referrerId, referredPhone, sentAt — máx 3 lifetime por identity |

---

### 1.7 Context Map — Strategic Relationships

| Relationship | Pattern | Notes |
|-------------|---------|-------|
| Catalogue → Identity | Downstream / Conformist | Catalogue dispara CatalogueSyncCompleted; Identity não tem dependência upstream |
| Negotiation → Identity | Customer / Supplier | Negotiation (customer) precisa de BuyerIdentity verificado antes de enviar Proposta. Identity define o contrato |
| Negotiation → Catalogue | Conformist | Negotiation lê dados do Listing (pisoDeNegociacao, price) mas não os modifica |
| Media Protection → Catalogue | Anti-Corruption Layer | Media Protection expõe API limpa para que Catalogue não se acople aos internals do FFmpeg/watermark |
| Security → Identity | Open Host Service | Security expõe ReferralGuard OHS que Identity chama para enforçar o limite de 3 referrals lifetime |
| Backoffice → All | Published Language | Backoffice publica API REST estável consumida por todos os outros contextos. Versionada com semver |

---

## PARTE 2 — UX Artifacts

### 2.1 User Personas

#### Persona 1 — Marcos, 34 | Comprador Pessoa Física

| | |
|-|-|
| **Goal** | Comprar um carro usado confiável para uso pessoal com orçamento de R$40k |
| **Context** | Navega no celular durante o almoço. Frequentemente em 4G instável |
| **Frustration** | Odeia criar contas antes de ver preços. Desconfia de listagens sem fotos reais |
| **Motivator** | Se sente empoderado fazendo uma contraproposta. Aprecia simulação de financiamento transparente |
| **Tech literacy** | Moderada. Usa WhatsApp diariamente. Confortável com pagamentos mobile |

#### Persona 2 — Renata, 41 | Lojista (Gestora de Concessionária)

| | |
|-|-|
| **Goal** | Avaliar veículos para repasse com margens competitivas. Precisa de dados rápidos para decisão |
| **Context** | Revisa listagens entre atendimentos. Espera informação de nível profissional (FIPE, condição) |
| **Frustration** | Tempo desperdiçado com vendedores não sérios. Quer sinalizar que é compradora qualificada imediatamente |
| **Motivator** | A seleção de categoria de comprador sinaliza sua intenção; vendedor prioriza seu contato |
| **Tech literacy** | Alta. Usa CRM e software de gestão de concessionária diariamente |

#### Persona 3 — Diego, 28 | Vendedor Autônomo / Repassador

| | |
|-|-|
| **Goal** | Encontrar veículos subprecificados para revender. Precisa de velocidade — as melhores oportunidades somem em horas |
| **Context** | Verifica o app de manhã cedo. Usa Wi-Fi em casa, 4G na rua |
| **Frustration** | Perde muito tempo em plataformas com listagens desatualizadas ou anúncios falsos |
| **Motivator** | Offline-First significa catálogo sempre carregado. Pode avaliar mesmo em áreas de sinal ruim (ferros-velhos, leilões) |
| **Tech literacy** | Alta. Power user. Provavelmente vai indicar o app para colegas (atingir o limite de 3 indicações) |

---

### 2.2 Information Architecture

| Section | Contents & Behaviour |
|---------|----------------------|
| **Home / Catálogo (Tab 1)** | Grid de veículos (imagem de capa, preço, nome). Barra de filtro/ordenação. Badge de sync connectivity-aware. Indicador offline se desatualizado |
| **Busca & Filtros (Tab 2)** | Busca full-text. Filtro por marca, modelo, ano, faixa de preço, condição. Funciona no SQLite/Hive local — totalmente offline |
| **Minhas Propostas (Tab 3)** | Lista de todas as Propostas e Interesses enviados. Badges de status (Enviada / Contato Realizado). Ponto de notificação quando status muda |
| **Detalhe do Anúncio (Modal/Stack)** | Galeria de fotos HD (Blur-Gate em 4G até toque). Tabela de specs. Player de Pitch de Áudio. Vídeo sob demanda com label de tamanho. Referência FIPE (se presente). Simulador de Financiamento. Botão "Negociar Agora". Trigger de X9 Report |
| **Martelinho Flow (Bottom Sheet)** | Fork de duas opções: Tenho Interesse / Fazer Oferta. Tela de oferta com sugestões Pill e campo livre. Intersticial de validação de identidade (se primeira vez). Tela de rejeição do Piso. Estado de sucesso Confete |
| **Onboarding Overlay (First Launch)** | Carrossel único (máx 3 slides). Skip disponível. Sem sign-in obrigatório. Desaparece após primeiro swipe |
| **Settings & Referral (Drawer)** | Toggle de tema. Painel de referral (até 3 contatos). Versão do app / status de sync |

---

### 2.3 User Journey Maps

#### Journey 1 — Navegação Anônima até Proposta (Marcos, PF)

| Stage | Action | Emotion | System Response |
|-------|--------|---------|-----------------|
| 1. App Open | Toca ícone em 4G fraco | Ansioso — vai carregar? | Carrega do DB local instantaneamente. Mostra badge de sync |
| 2. Browse Grid | Rola grid de veículos, vê imagens de capa | Curioso, positivo — está rápido! | Imagens de capa exibidas; imagens HD com Blur-Gate |
| 3. Open Listing | Toca em uma listagem que gosta | Engajado | Detalhe da listagem abre. Toca na foto → HD carrega sob demanda |
| 4. Listen to Pitch | Toca no pitch de áudio | Confiança crescendo | Áudio toca com stream protegido por ruído |
| 5. Run Simulator | Abre simulador de financiamento | Tranquilizado pelos números | Calcula localmente; sem chamada de rede |
| 6. Decide to Offer | Toca "Negociar Agora" → "Fazer Oferta" | Empolgado, leve hesitação | Bottom sheet Martelinho abre com sugestões pill |
| 7. Enter Offer | Toca pill -10% | Brincalhão, sente controle | Valor preenchido. Botão Submit ativa |
| 8. Identity Gate | Solicitado a verificar via WhatsApp | Breve fricção — esperada | OTP enviado. Seletor de BuyerProfile exibido |
| 9. Select Profile | Escolhe "Pessoa Física" | Se sente categorizado, compreendido | Lead record criado com categoria |
| 10. Confete | Proposta enviada | Alegria! | Animação Confete. Disclaimer não-vinculante exibido |

#### Journey 2 — Vendedor Backoffice: Upload de Listagem & Monitor de Lead

| Stage | Seller Action & System Response |
|-------|--------------------------------|
| 1. Login to Backoffice | Vendedor acessa o dashboard admin. Vê contagem de leads por tipo |
| 2. Create New Listing | Preenche detalhes do veículo, preço, FIPE opcional, define Piso de Negociação |
| 3. Upload Media | Faz upload de fotos (auto-conversão WebP), vídeo (overlay FFmpeg injetado), grava pitch de áudio (ruído injetado) |
| 4. Publish & Sync Trigger | Toca "Publicar". Servidor incrementa hash version. Clientes conectados recebem evento CatalogueSyncRequired |
| 5. Monitor Leads | Dashboard mostra novas Propostas e Interesses. Filtrado por BuyerProfile (ex: Lojista primeiro) |
| 6. Contact Lead | Vendedor toca em um lead → deeplink WhatsApp abre. Status do lead auto-atualiza para "Contato Realizado" |
| 7. Review X9 Reports | Vendedor revisa reports de fraude enviados por compradores |

---

### 2.4 Wireframe Annotations

#### Screen A — Home / Catálogo Grid

| Element | Annotation |
|---------|------------|
| Header Bar | Logo à esquerda, badge de Sync Status à direita (ponto: verde=sincronizado, amarelo=sincronizando, vermelho=desatualizado) |
| Connectivity Banner | Exibido abaixo do header se offline. "Modo Offline — Dados de [data]". Dispensável |
| Search Bar | Fixo abaixo do header. Toque entra na stack de Busca & Filtros |
| Vehicle Card | Imagem de capa (16:9, WebP). Nome do veículo + ano. Preço em badge pill #E9FF00 em #021E73. Botão ghost "Negociar" |
| Grid Layout | 2 colunas em phones <400dp; modo lista 1 coluna em telas estreitas |
| Bottom Tab Bar | Catálogo / Busca / Propostas. Tab ativa usa cor de destaque (#E9FF00) |

#### Screen B — Listing Detail

| Element | Annotation |
|---------|------------|
| Photo Gallery | Carrossel swipeable full-width. Imagens HD carregadas sob demanda em 4G (Blur-Gate até toque). Zoom HD no pinch |
| Vehicle Title & Price | Título em negrito. Preço em badge grande #E9FF00. Linha de referência FIPE abaixo se presente |
| Pitch de Áudio | Card com visualizador de forma de onda. Botão play. Label de duração. Atribuição "Por [Seller Name]" |
| Specs Table | Ano, km, combustível, câmbio, cor, condição. Grid compacto de 2 colunas |
| Simulador de Financiamento | Card recolhível. Input: entrada, meses. Output: parcela mensal. Funciona offline |
| Video Card | Thumbnail + ícone play. Em 4G: label "Ver vídeo — 12MB" visível antes do play. Em Wi-Fi: pré-carregado |
| Negociar Agora Button | Full-width, fundo #E9FF00, texto #021E73. Fixo no bottom da tela (sticky) |
| X9 Report Link | Link de texto terciário no final: "Denunciar anúncio falso". Abre modal |

#### Screen C — Martelinho Flow (Bottom Sheet Stack)

| Element | Annotation |
|---------|------------|
| Sheet 1 — Choose Path | Dois cards grandes: "Tenho Interesse" (ícone info, outline azul) e "Fazer Oferta" (ícone martelo, azul preenchido) |
| Sheet 2 — Offer Entry | Linha de pills: -5% / -10% / -15% (auto-calculado). Campo de input livre abaixo. Preço atual da listagem como referência |
| Piso Rejection State | Banner vermelho: "Valor muito abaixo da pretensão do vendedor." Botão Submit desabilitado. Pill mais próxima sugerida |
| Identity Interstitial | Exibido apenas na primeira Proposta/Interesse. Campo de telefone + seletor de canal (WhatsApp / SMS). Entrada OTP |
| BuyerProfile Selector | 4 cards de radio grandes: PF / Vendedor Autônomo / Lojista / Investidor. Cada um com ícone e descrição de uma linha |
| Confete Screen | Overlay verde full-sheet. Animação de partículas Confete. Título "Proposta enviada!". Disclaimer: "Esta proposta não constitui compromisso legal." CTA: "Ver minhas propostas" |

---

### 2.5 UX Design Principles & Patterns

| Principle | Implementation | Rationale |
|-----------|---------------|-----------|
| Zero-Friction Entry | Sem login wall ao abrir. Navegação livre | Reduz abandono causado por ansiedade de identidade. Login acionado apenas no momento de troca de valor |
| Post-Intent Validation | Gate de identidade aparece somente após sinalização de intenção (Proposta/Interesse) | Aumenta motivação para completar verificação — usuário já está investido |
| Progressive Disclosure | Specs, simulador e pitch são recolhíveis abaixo do fold | CTA principal ("Negociar Agora") permanece acima do fold sem poluição visual |
| Connectivity-Aware UI | UI reflete estado da rede: Blur-Gate, badges de sync, labels de tamanho de dados | Previne confusão com conteúdo ausente; constrói confiança através da transparência |
| Gamification via Anchor | Sugestões pill enquadram a oferta relativa ao preço pedido | Efeito de ancoragem: usuários são guiados a ofertas dentro da faixa negociável, reduzindo rejeições pelo Piso |
| Trust Signals | Fotos com watermark, pitch de áudio do vendedor, referência FIPE | Marcadores de autenticidade diferenciam de listagens falsas. Áudio humaniza a transação |
| Outcome Celebration | Confete no sucesso da Proposta | Reforço positivo aumenta probabilidade de re-engajamento e indicações |
| Transparent Constraints | Disclaimer não-vinculante na tela Confete | Clareza legal previne disputas pós-oferta e arrependimento do comprador |

---

## PARTE 3 — Behaviour-Driven Development (BDD)

> Todos os cenários seguem a sintaxe Gherkin: Feature, Background, Scenario, Given / When / Then / And / But.

---

### Feature 3.1 — Offline-First Catalogue Browsing

**Scenario 1.1 — App opens with stale cache on Wi-Fi**
```gherkin
Given the user has previously opened the app and a catalogue was cached locally
And   the local hash version is 'v3' and the server hash is 'v5'
And   the device is connected to Wi-Fi
When  the app is launched
Then  the catalogue grid is rendered immediately from the local cache
And   a sync badge shows 'Atualizando catálogo…'
And   the app downloads the full Pacotão JSON including HD images
And   the sync badge changes to 'Catálogo atualizado' upon completion
```

**Scenario 1.2 — App opens with stale cache on 4G**
```gherkin
Given the local hash is 'v3' and the server hash is 'v5'
And   the device is connected via 4G
When  the app is launched
Then  the app downloads only the JSON manifest and cover images
And   HD images are NOT pre-downloaded
And   vehicle cards display cover images correctly
```

**Scenario 1.3 — User opens a listing detail on 4G (Blur-Gate)**
```gherkin
Given the app is running on a 4G connection
And   HD images for listing 'HND-001' have not been pre-loaded
When  the user taps on listing 'HND-001'
Then  the gallery displays a blurred low-res image
And   an overlay reads 'Toque para carregar foto em HD'
When  the user taps the blurred image
Then  the HD image is fetched on-demand and replaces the blur
```

**Scenario 1.4 — App is fully offline**
```gherkin
Given the device has no active internet connection
And   a catalogue was previously cached on 'Jan 10'
When  the user opens the app
Then  the catalogue grid loads instantly from local storage
And   a banner displays 'Modo Offline — Dados de 10 de Janeiro'
And   the search and filter functions operate on the local database
But   the 'Negociar Agora' button is disabled with tooltip
      'Necessário conexão para enviar proposta'
```

---

### Feature 3.2 — User Identification (Post-Intent Validation)

**Scenario 2.1 — First-time offer triggers identity gate**
```gherkin
Given the user has not previously verified their identity
And   the user is on the listing detail for 'HND-001'
When  the user taps 'Negociar Agora' and selects 'Fazer Oferta'
Then  the Martelinho offer entry sheet is displayed
When  the user taps 'Enviar Proposta'
Then  an identity verification interstitial is shown
And   the user can choose 'WhatsApp' or 'SMS' as the verification channel
```

**Scenario 2.2 — Successful WhatsApp OTP verification**
```gherkin
Given the identity interstitial is shown
And   the user enters phone number '+55 11 9XXXX-XXXX'
And   the user selects 'WhatsApp'
When  the user taps 'Enviar Código'
Then  an OTP is dispatched via WhatsApp to the provided number
When  the user enters the correct 6-digit code
Then  the identity is verified and BuyerProfile selector is shown
```

**Scenario 2.3 — Buyer selects 'Lojista' profile**
```gherkin
Given the identity has been verified for phone '+55 11 9XXXX-XXXX'
And   the BuyerProfile selector is displayed
When  the user selects 'Lojista (Gerente/Gestor)'
Then  the lead record is created with category = 'Lojista'
And   the lead appears in the Backoffice filtered view under 'Lojista'
```

**Scenario 2.4 — Returning verified user skips identity gate**
```gherkin
Given the user has previously verified their identity in this session
When  the user submits a second Proposta on a different listing
Then  the identity interstitial is NOT shown
And   the Proposta is submitted directly using the BuyerProfile on record
```

---

### Feature 3.3 — Negotiation Flow (The Martelinho)

**Scenario 3.1 — Successful offer using pill suggestion**
```gherkin
Given the listing 'HND-001' has a price of R$50.000 and a Piso of R$44.000
And   the user is verified with profile 'PF'
When  the user taps 'Fazer Oferta'
And   the user taps the '-10%' pill
Then  the offer input is populated with 'R$ 45.000'
When  the user taps 'Enviar Proposta'
Then  the offer value (R$45.000) is compared against the Piso (R$44.000) client-side
And   the check passes and the Proposta is submitted to the backend
And   the Confete animation is displayed with the non-binding disclaimer
```

**Scenario 3.2 — Offer rejected by Piso de Negociação**
```gherkin
Given the listing 'HND-001' has a price of R$50.000 and a Piso of R$44.000
When  the user manually types an offer of 'R$ 40.000'
And   the user taps 'Enviar Proposta'
Then  the PisoGuard domain service blocks the submission
And   a red banner shows 'Valor muito abaixo da pretensão do vendedor'
And   NO network request is made to the backend
And   the nearest valid pill (-15% = R$42.500) is highlighted as suggestion
```

**Scenario 3.3 — User submits 'Tenho Interesse'**
```gherkin
Given the user is verified and on listing 'HND-001'
When  the user taps 'Negociar Agora' and selects 'Tenho Interesse'
And   the user submits the interest form
Then  an InterestLeadCreated event is fired
And   the status screen shows status 'Enviada'
And   the lead appears in the Backoffice under 'Interesse' filter
```

**Scenario 3.4 — Tracking proposal status**
```gherkin
Given the user has submitted a Proposta for listing 'HND-001'
And   the proposal has status 'Enviada'
When  the seller opens the lead in Backoffice and taps the WhatsApp deeplink
Then  a PropostaStatusUpdated event fires with status 'ContatoRealizado'
And   the buyer's 'Minhas Propostas' screen updates the badge to 'Contato Realizado'
```

---

### Feature 3.4 — Financing Simulator

**Scenario 4.1 — Simulation runs offline**
```gherkin
Given the device has no internet connection
And   the seller has configured a monthly rate of 1.49% in the Backoffice
And   the rate was synced during the last successful Pacotão download
When  the user opens the Simulador on listing 'HND-001' (R$50.000)
And   the user enters entry amount 'R$10.000' and selects '36 months'
Then  the monthly instalment is calculated locally using the cached rate
And   the result is displayed without any network call
```

**Scenario 4.2 — Rates update on next sync**
```gherkin
Given the seller updates the monthly rate from 1.49% to 1.79% in the Backoffice
And   a new catalogue hash is generated
When  the buyer's app performs the next sync
Then  the updated rate is included in the Pacotão JSON
And   subsequent simulator calculations use the new 1.79% rate
```

---

### Feature 3.5 — Media Consumption

**Scenario 5.1 — Video play on 4G shows file size label**
```gherkin
Given the device is connected via 4G
And   listing 'HND-001' has a video of 18MB
When  the user views the listing detail
Then  the video card displays 'Ver vídeo — 18MB' before the user taps play
When  the user taps play
Then  the video begins streaming
```

**Scenario 5.2 — Video on Wi-Fi plays without size prompt**
```gherkin
Given the device is connected via Wi-Fi
When  the user views the listing detail for a vehicle with a video
Then  the video is pre-fetched as part of the full sync
And   the file size label is NOT shown
And   the video plays immediately on tap
```

**Scenario 5.3 — Audio pitch plays with noise injection**
```gherkin
Given the seller has uploaded a pitch audio for 'HND-001'
And   the server has injected inaudible noise into the audio file
When  the user taps the audio play button
Then  the audio plays normally for the human listener
And   the served file contains embedded noise that disrupts AI voice-cloning
And   the file is served as an obfuscated .dat extension
```

---

### Feature 3.6 — Security & Anti-Spam

**Scenario 6.1 — Referral limit enforced at 3 contacts**
```gherkin
Given the user 'Marcos' (identityId: U001) has already referred 3 contacts
When  Marcos attempts to refer a 4th contact via the Referral panel
Then  the ReferralLimitExceeded event fires
And   the input field is disabled
And   a message reads 'Você atingiu o limite de 3 indicações'
And   no referral message is dispatched
```

**Scenario 6.2 — User submits X9 fraud report**
```gherkin
Given the user is viewing listing 'HND-001'
And   the user taps 'Denunciar anúncio falso'
When  the X9 modal opens
And   the user selects 'OLX' from the platform combobox
And   the user taps 'Enviar Denúncia'
Then  an X9ReportSubmitted event fires with {platform: 'OLX', listingId: 'HND-001'}
And   a confirmation toast reads 'Denúncia enviada. Obrigado!'
```

**Scenario 6.3 — Uploaded image receives watermark**
```gherkin
Given the seller uploads a JPEG photo for listing 'HND-001' in the Backoffice
When  the MediaUploadCompleted event fires
Then  the server converts the image to WebP format
And   a semi-transparent watermark is applied server-side
And   the watermarked WebP URL is stored as the canonical asset URL
And   the original JPEG is not publicly accessible
```

---

### Feature 3.7 — Backoffice Lead Management

**Scenario 7.1 — Lead dashboard shows segmented view**
```gherkin
Given the seller is logged into the Backoffice
And   there are 12 leads: 5 PF Propostas, 4 Lojista Propostas, 2 Interesses, 1 Investidor
When  the seller opens the Leads dashboard
Then  leads are grouped by type: 'Propostas' (9), 'Interesses' (2), 'Outros' (1)
And   a filter chip row shows: Todos | PF | Lojista | Vendedor Autônomo | Investidor
And   the default view shows all Propostas sorted by most recent
```

**Scenario 7.2 — Seller contacts lead via WhatsApp deeplink**
```gherkin
Given the seller taps on a Proposta from 'Renata' (Lojista) for 'HND-001'
When  the seller taps 'Contatar via WhatsApp'
Then  WhatsApp opens with a pre-filled message referencing the vehicle and offer
And   the lead status updates to 'ContatoRealizado' in the backend
And   a BackofficeLeadViewed event is fired
```

**Scenario 7.3 — Seller forces client re-sync**
```gherkin
Given the seller has updated 3 listings
When  the seller taps 'Publicar e Forçar Sincronização'
Then  the server increments the global catalogue hash
And   all connected clients receive CatalogueSyncRequired within 30 seconds
And   clients on Wi-Fi download the full updated Pacotão with HD images
And   clients on 4G download the JSON and cover images only
```

---

## PARTE 4 — Instrumentation & Metrics Strategy

### 4.1 North Star & Key Product Metrics

| Metric | Type | Definition & Why It Matters |
|--------|------|-----------------------------|
| **Time to Value (TTV)** | North Star | Tempo entre `app_opened` e o primeiro `negotiation_initiated`. Valida diretamente a filosofia Zero-Friction. TTV alto indica fricção no catálogo ou tela de detalhe antes do engajamento |
| **Activation Rate** | Funnel Health | % de usuários que completam `identity_verified` após `negotiation_initiated`. Fórmula: identity_verified / negotiation_initiated. Mede o gate de identidade pós-intenção — o maior risco de conversão do MVP |
| **Time to Convert (TTC)** | Funnel Depth | Tempo entre `identity_verified` e `proposta_submitted`. Indica se o Martelinho em si está causando hesitação após o usuário já estar comprometido |

---

### 4.2 Analytics Event Map

| Analytics Event | Maps to Domain Event | Key Properties |
|----------------|---------------------|----------------|
| `app_opened` | — (App lifecycle) | session_id, connectivity_type, catalogue_version, is_first_open |
| `negotiation_initiated` | `NegotiationInitiated` | listing_id, channel_type, time_since_app_open_ms |
| `identity_verified` | `IdentityVerified` | verification_channel, buyer_profile, time_since_negotiation_initiated_ms |
| `proposta_submitted` | `PropostaSubmitted` | listing_id, offer_value, pill_used, time_since_identity_verified_ms |
| `piso_rejection` | `PropostaRejectedByPiso` | listing_id, offered_value, piso_value, delta_percent |
| `blur_gate_triggered` | `MediaBlurGateTriggered` | listing_id, media_type |
| `x9_report_submitted` | `X9ReportSubmitted` | platform, listing_id — sem PII |

> **Regra de privacidade:** Nenhum PII em propriedades de evento. Telefones e nomes nunca são enviados para analytics. Apenas `session_id` e `listing_id` anonimizados.

---

### 4.3 Deferred Metrics — Post-MVP

| Metric | Category | Reason for Deferral |
|--------|----------|---------------------|
| Lead Velocity Rate (LVR) | Growth / Sales | Requer volume semanal consistente. Monitore manualmente no Backoffice por enquanto |
| Onboarding Completion Rate | UX Quality | Carrossel de 3 slides dispensável tem risco mínimo de abandono. Revisitar se onboarding for redesenhado |
| Time to Market (TTM) | Ops / Dev | Relevante quando um time publica listagens. Para vendedor solo, é apenas o fluxo de upload |
| Retention D1/D7/D30 | Engagement | Requer base de usuários grande o suficiente para análise de coorte |
| Piso Rejection Rate por Listing | Pricing Strategy | Útil para o vendedor recalibrar valores de Piso. Surfacear no Backoffice v2 como contador por listagem |
| Referral Conversion Rate | Growth | Requer lógica de atribuição de referral além do enforcement do limite de 3 contatos |
| TTI / FMP (Performance) | App Quality | Disponível gratuitamente via Firebase Performance Monitoring SDK sem instrumentação customizada |

---

### 4.4 Implementation Notes

| | |
|-|-|
| **Tool** | Firebase Analytics — free tier, sem backend necessário, filas de eventos offline nativas |
| **Pattern** | Observer no domain event bus — chamadas de analytics são side effects, nunca dentro da lógica de negócio |
| **Offline Behaviour** | Firebase SDK enfileira eventos localmente e faz flush ao restaurar conectividade — consistente com arquitetura Offline-First |
| **Privacy** | Sem PII nas propriedades de evento. Telefones e nomes nunca são enviados para analytics |
| **Dashboard** | Configurar `app_opened` → `negotiation_initiated` → `identity_verified` → `proposta_submitted` como funil de conversão único no Firebase |

---

*Documento vivo — atualizar ao longo das fases de UX e desenvolvimento.*
*Todos os modelos de domínio, personas, jornadas, cenários BDD e estratégia de métricas derivam da especificação do produto.*
