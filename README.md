# 랜덤 건물 부수기 (Random Building Crusher)

고전 건물 부수기 + 뱀서류 레벨업 시스템의 Unity 2D 모바일 게임.

- **Unity 버전**: 6000.3.x (Unity 6.3)
- **장르**: 건물 파괴 액션 + 로그라이크 성장
- **플랫폼**: 모바일 (앱인토스 미니게임)

---

## 실행 방법

### 1단계 — Unity Hub에서 열기

1. Unity Hub 실행
2. `Open` 클릭
3. `random-building-crusher` 폴더 선택
4. Unity 6000.3.x 버전으로 열기

### 2단계 — 씬 만들기

```
File > New Scene > Empty > Create
File > Save As > Assets/Scenes/Game.unity
```

### 3단계 — 씬 자동 생성

Unity 상단 메뉴:
```
BuildingCrusher > Setup Scene
```

### 4단계 — 실행

▶ Play 클릭 → 타이틀 화면 → "시작" 클릭

---

## 조작법

| 조작 | 기능 |
|------|------|
| 화면 터치/클릭 | 캐릭터 이동 |
| 건물 근처 도달 | 자동 공격 |
| 레벨업 카드 터치 | 능력 선택 |

---

## 게임 규칙

- 각 스테이지에 건물 1개 (세로 다층)
- 아래층부터 순서대로 파괴
- 스테이지 클리어: 30초 기본 + 잔여 시간 이월
- 게임 오버: 시간 초과 또는 체력 0
- 레벨업: 경험치 기반, 3개 중 1개 선택 (능력치 20개 + 기술 10개)
- 랭킹: 글로벌 점수 기준 (Plan 2에서 구현)

---

## 프로젝트 구조

```
Assets/Scripts/
├── Core/           GameEngine.cs, GameState.cs, GameSnapshot.cs
├── Data/           GameData.cs, BuildingDefs.cs, AbilityDefs.cs, SkillDefs.cs
├── Editor/         SceneSetup.cs (씬 자동 생성 메뉴)
├── Entities/       BuildingView, CharacterView, HazardView, FloatingTextView
├── Managers/       GameManager.cs
├── Rendering/      BuildingRenderer.cs, CharacterRenderer.cs, HazardRenderer.cs, EffectsManager.cs
└── UI/             UIManager.cs, LevelUpModal.cs
```
