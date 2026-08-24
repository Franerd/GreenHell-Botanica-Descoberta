# Botânica Descoberta

Mod para Green Hell que exibe nomes comuns e científicos nas entradas botânicas já descobertas pelo jogador.

## Características

- 85 `ItemIDs` distribuídos em 25 espécies ou grupos botânicos.
- Nomes específicos para folhas, flores, frutos, sementes, raízes e cogumelos.
- Modos de exibição comum, científico ou combinado.
- Traduções em português do Brasil, inglês e espanhol, selecionadas automaticamente pelo idioma do Green Hell.
- Inglês como fallback para os demais idiomas do jogo.
- Funciona localmente para host e cliente em partidas cooperativas.
- Não desbloqueia páginas, não altera progresso e não grava nomes no save.

## Comandos

```text
botanica status
botanica comum
botanica cientifico
botanica ambos
botanica aplicar
botanica cogumelos
```

Os comandos também aceitam `common`, `scientific`, `both`, `apply`, `mushrooms`, `comun` e `hongos`.

## Instalação

Copie o arquivo `.ghmod` da release para a pasta `mods` do Green Hell e compile/ative pelo ModLoader.

## Compatibilidade

- Green Hell 2.9.5
- Testado como host e cliente.

## Desenvolvimento

O ModCompiler compila os arquivos `BotanicaDescoberta.cs` e `BotanicaCatalog.cs`. O arquivo `catalogo-botanica-descoberta.json` documenta a origem lógica dos nomes, o agrupamento das espécies e os níveis de confiança das identificações.

## Licença

GNU Affero General Public License v3.0. Consulte [LICENSE](LICENSE).
