# Spot The Difference - File Structure

## Frontend (Spot_The_DifferenceFrontend)

```
Spot_The_DifferenceFrontend/
├── public/
│   ├── tn.png
│   └── vite.svg
├── src/
│   ├── assets/
│   │   ├── Images/
│   │   │   ├── easy2after.png
│   │   │   ├── easy2before.png
│   │   │   ├── easyafter.png
│   │   │   ├── easybefore.png
│   │   │   ├── hardafter.png
│   │   │   ├── hardbefore.png
│   │   │   ├── medium2after.png
│   │   │   ├── medium2before.png
│   │   │   ├── mediumafter.png
│   │   │   └── mediumbefore.png
│   │   └── react.svg
│   ├── components/
│   │   ├── Page.tsx
│   │   └── Timer.tsx
│   ├── data/
│   │   ├── gameState.ts
│   │   └── levels.ts
│   ├── hooks/
│   │   └── TimerCountUp.tsx
│   ├── pages/
│   │   ├── AdminPage.tsx
│   │   ├── Differences.tsx
│   │   ├── Question.tsx
│   │   ├── Results.tsx
│   │   ├── ResultsPage.tsx
│   │   ├── StartImage.tsx
│   │   └── StartMenu.tsx
│   ├── Styles/
│   │   └── Global.css
│   ├── App.css
│   ├── App.tsx
│   ├── index.css
│   ├── main.tsx
│   └── translations.ts
├── eslint.config.js
├── index.html
├── package.json
├── package-lock.json
├── README.md
├── tsconfig.app.json
├── tsconfig.json
├── tsconfig.node.json
├── vite.config.ts
└── yarn.lock
```

## Backend (Spot_The_DifferenceBackend)

```
Spot_The_DifferenceBackend/
├── Spot_The_Difference.API/
│   ├── Controllers/
│   │   ├── AdminController.cs
│   │   ├── GameController.cs
│   │   └── TestDbController.cs
│   ├── Properties/
│   │   └── launchSettings.json
│   ├── wwwroot/
│   │   └── images/
│   │       └── [66 PNG image files]
│   ├── appsettings.Development.json
│   ├── appsettings.json
│   ├── Program.cs
│   ├── Spot_The_Difference.API.csproj
│   └── Spot_The_Difference.API.http
│
├── Spot_The_Difference.Contracts/
│   ├── Requests/
│   │   ├── CreateRoundRequest.cs
│   │   ├── GuessRequest.cs
│   │   └── StartGameRequest.cs
│   ├── Responses/
│   │   ├── AnswerResponse.cs
│   │   ├── GuessResponse.cs
│   │   └── StartGameResponse.cs
│   └── Spot_The_Difference.Contracts.csproj
│
├── Spot_The_Difference.Domain.Model/
│   ├── DTOs/
│   │   └── LevelDTO.cs
│   ├── Difference.cs
│   ├── DifferenceOption.cs
│   ├── Player.cs
│   ├── PlayerRound.cs
│   └── Spot_The_Difference.Domain.Model.csproj
│
├── Spot_The_Difference.Domain.Services/
│   ├── AdminService.cs
│   ├── GameService.cs
│   ├── LevelService.cs
│   └── Spot_The_Difference.Domain.Services.csproj
│
├── Spot_The_Difference.Persistence/
│   ├── Interfaces/
│   │   ├── IPlayerRepository.cs
│   │   ├── IPlayerRoundRepository.cs
│   │   ├── IQuestionRepository.cs
│   │   └── IRoundRepository.cs
│   ├── Repositories/
│   │   ├── PlayerRepository.cs
│   │   ├── PlayerRoundRepository.cs
│   │   ├── QuestionRepository.cs
│   │   └── RoundRepository.cs
│   ├── AppDbContext.cs.cs
│   └── Spot_The_Difference.Persistence.csproj
│
├── Spot_The_Difference.Persistence.Entities/
│   ├── MijnMap/
│   │   ├── Answeroption.cs
│   │   ├── Difference.cs
│   │   ├── Differenceoption.cs
│   │   ├── Difficulty.cs
│   │   ├── Image.cs
│   │   ├── Player.cs
│   │   ├── Playerround.cs
│   │   ├── Question.cs
│   │   ├── Round.cs
│   │   └── SpotthedifferencedbContext.cs
│   └── Spot_The_Difference.Persistence.Entities.csproj
│
└── Spot_The_DifferenceBackend.sln
```

## Project Structure Overview

### Frontend (React + TypeScript + Vite)
- **src/pages/**: React page components
- **src/components/**: Reusable React components
- **src/hooks/**: Custom React hooks
- **src/data/**: Data models and state management
- **src/Styles/**: CSS styling files
- **translations.ts**: Internationalization translations (en, nl, fr)

### Backend (.NET 9.0 - Clean Architecture)
- **Spot_The_Difference.API/**: Web API layer (Controllers, Program.cs)
- **Spot_The_Difference.Contracts/**: DTOs for API requests/responses
- **Spot_The_Difference.Domain.Model/**: Domain models and DTOs
- **Spot_The_Difference.Domain.Services/**: Business logic services
- **Spot_The_Difference.Persistence/**: Repository pattern implementation
- **Spot_The_Difference.Persistence.Entities/**: Entity Framework database entities

