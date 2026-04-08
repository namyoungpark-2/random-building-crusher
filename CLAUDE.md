# CLAUDE.md — 랜덤 건물 부수기 Unity 프로젝트

## 한 줄 요약

고전 건물 부수기 + 뱀서류 레벨업 시스템의 Unity 2D 모바일 게임.
화면 터치로 캐릭터 이동, 건물 근처 자동 공격, 층별 파괴, 경험치 기반 레벨업.

## 핵심 아키텍처

### 데이터 흐름

```
TouchInput → GameEngine.ProcessTouch(GameState, x, y)
           → GameEngine.UpdateGame(GameState, dt)
               ├─ UpdateCharacter (이동, 공격)
               ├─ UpdateBuilding (HP, 층 파괴)
               ├─ UpdateHazards (낙하물, 반격)
               ├─ UpdateEnvironment (지진, 폭풍)
               ├─ UpdateExp (레벨업 체크)
               └─ UpdateTimer (시간/체력)
           → GameSnapshot.Create(GameState) (읽기 전용)
           → Renderers.Render(snap)
           → UIManager.UpdateGameUI(snap)
```

### 파일별 역할

핵심 3파일:
1. `Assets/Scripts/Data/` — 건물/능력치/기술 정의
2. `Assets/Scripts/Core/GameEngine.cs` — 전체 게임 로직
3. `Assets/Scripts/Core/GameState.cs` — 상태 데이터

### 네임스페이스

```
BuildingCrusher.Core       → GameEngine, GameState, GameSnapshot
BuildingCrusher.Data       → GameData, BuildingDefs, AbilityDefs, SkillDefs
BuildingCrusher.Managers   → GameManager
BuildingCrusher.Rendering  → BuildingRenderer, CharacterRenderer, HazardRenderer, EffectsManager
BuildingCrusher.UI         → UIManager, LevelUpModal
BuildingCrusher.Entities   → BuildingView, CharacterView, HazardView, FloatingTextView
BuildingCrusher.Editor     → SceneSetup (#if UNITY_EDITOR)
```

## 코드 수정 시 주의사항

- GameState를 직접 수정하지 말고 GameEngine 메서드를 통해서만 변경
- GameData, BuildingDefs, AbilityDefs, SkillDefs는 static readonly
- SceneSetup은 #if UNITY_EDITOR 안에 있어야 함
