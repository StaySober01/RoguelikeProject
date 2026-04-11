# Roguelike Project

> **턴제 로그라이크 포트폴리오 프로젝트**

---

## 1. Overview

### 프로젝트 소개
**Roguelike Project**는  
플레이어가 전투 중 얻는 다양한 효과와 상호작용을 조합하여,  
중후반에 강력한 시너지 빌드를 완성하는 것을 핵심 재미로 삼는  
**턴제 로그라이크 게임**입니다.

이 프로젝트는 단순한 전투 구현이 아니라,  
**시너지 기반 시스템 설계 및 구현 능력**을 보여주기 위한  
Unity 포트폴리오 프로젝트로 개발하고 있습니다.

### 개발 목적
- 게임업계 취업용 핵심 포트폴리오 제작
- 전투 시스템 / 상태이상 / 패시브 / 이벤트 기반 구조 설계 능력 증명
- 확장 가능한 Unity 아키텍처 설계 및 구현 연습

---

## 2. Core Fun

### 핵심 재미
이 프로젝트의 핵심 재미는 다음과 같습니다:

> 개별 효과가 서로 연결되며 하나의 시너지 엔진으로 완성되는 과정에서 발생하는 전략적 쾌감

### 설계 철학
- 개별 효과는 단독으로는 강하지 않아야 한다
- 하지만 여러 효과가 연결되면 강한 폭발력을 가져야 한다
- 플레이어는 “좋은 선택”이 아니라 **잘 엮인 선택**을 했을 때 보상을 받아야 한다
- 강한 단일 아이템보다, **조합된 시스템 전체가 강해지는 구조**를 목표로 한다

---

## 3. Current Features

### 전투 시스템
- [x] 턴 기반 전투 루프
- [x] 에너지 기반 행동 시스템
- [x] 플레이어 / 적 유닛 구조
- [x] 전투 상태 관리 (PlayerTurn / EnemyTurn / Win / Lose)

### 카드 시스템
- [x] ScriptableObject 기반 카드 데이터 구조
- [x] Effect 조합 기반 카드 설계 (List<ICardEffect>)
- [x] 조건 / 동적 계산 / 전용 Effect 지원
- [x] 신규 카드 코드 수정 없이 추가 가능

### 상태이상 시스템
- [x] Poison / Burn / Vulnerable 구현
- [x] 상태이상 간 시너지 구조
- [x] 상태이상 기반 전투 설계

### 유물 시스템
- [x] 이벤트 기반 유물 효과 구조 (IRelicEffect)
- [x] ScriptableObject 기반 유물 데이터 구조
- [x] 전투 이벤트 트리거 기반 효과 발동
- [x] 턴 제한 / 재귀 방지 처리

### 보상 시스템
- [x] 전투 종료 후 카드 선택
- [x] 유물 보상 선택
- [x] 중복 유물 방지

---

## 4. Planned Features

### MVP 목표
이 포트폴리오 프로젝트의 MVP 범위는 다음과 같습니다.

#### 전투 시스템
- [x] 상태이상 시스템
- [x] 패시브 / 유물 시스템
- [x] 전투 종료 후 보상 선택
- [ ] 적 행동 패턴 다양화
- [ ] 시너지 빌드 분기 구조

#### 전투 내 빌드 방향 예시
- [ ] 중독 누적 빌드
- [ ] 감전 연쇄 빌드
- [ ] 점화 폭발 빌드

#### 디버그 / UX
- [ ] 전투 로그 UI
- [ ] 스킬 버튼 추가
- [ ] 현재 상태이상 표시 UI

---

## 5. Tech Stack

### Engine / Language
- **Unity 6000.3.12f1 (LTS)**
- **C#**

### Tools
- Git / GitHub
- Visual Studio
- TextMeshPro

---

## 6. Project Structure

```text
Assets/
├── _Project/
│   ├── Art/
│   ├── Audio/
│   ├── Data/
│   ├── Prefabs/
│   ├── Scenes/
│   ├── Scripts/
│   │   ├── Core/
│   │   ├── Battle/
│   │   ├── Cards/
│   │   ├── Effects/
│   │   ├── Units/
│   │   ├── StatusEffects/
│   │   ├── Passives/
│   │   ├── UI/
│   │   └── Utils/
│   └── SO/
│       ├── Cards/
│       └── Relics/
```

구조 설계 의도
- Core: 전역 게임 흐름 및 핵심 매니저
- Battle: 전투 루프 및 턴 진행 로직
- Units: 플레이어 / 적 유닛 관련 데이터 및 동작
- StatusEffects: 상태이상 관련 로직
- Passives: 패시브 / 유물 / 트리거 효과
- UI: 전투 UI 및 디버그 UI
- SO: 데이터 기반 확장을 위한 ScriptableObject

## 7. Key Design Goals

이 프로젝트에서 특히 보여주고 싶은 역량은 다음과 같습니다.

### 1) 객체 간 상호작용 설계

단일 효과가 아니라,
여러 객체와 시스템이 서로 참조하고 반응하는 구조를 설계하는 것

### 2) 이벤트 기반 전투 구조

전투 흐름을 단순한 하드코딩이 아니라,
확장 가능한 턴 / 행동 / 효과 처리 흐름으로 정리하는 것

### 3) 데이터 기반 확장성

상태이상, 패시브, 행동, 보상 등을
추후 데이터 중심으로 확장 가능한 구조로 만드는 것

### 4) “시너지”를 코드 구조로 표현하는 것

이 프로젝트의 가장 중요한 목표는
단순히 수치를 올리는 것이 아니라,
효과 간 연결과 누적이 재미가 되는 구조를 구현하는 것입니다.

## System Design

### 카드 시스템 (Effect 기반 구조)

CardDataSO
 └─ List<CardEffectDataSO>
        └─ Runtime ICardEffect 생성

- 카드 로직을 데이터로 정의
- 조건 / 동적 수치 / 전용 효과 지원
- 코드 수정 없이 카드 확장 가능

---

### 상태이상 시스템

Unit
 └─ UnitStatusData
        └─ Dictionary<StatusEffectType, int>

- 상태이상 처리 전담 Controller 분리
- 상태이상 간 시너지 중앙 관리

---

### 유물 시스템 (이벤트 기반)

RelicDataSO
 └─ List<RelicEffectDataSO>
        └─ Runtime IRelicEffect 생성

- 이벤트 기반 트리거 구조
- 카드 시스템과 동일한 데이터 기반 구조
- 전투 시스템과 느슨한 결합

---

### 핵심 설계 포인트

- 데이터 기반 구조 (SO)
- 이벤트 기반 상호작용
- 확장성 (코드 수정 최소화)
- 상태 스냅샷 기반 조건 처리

## 8. Development Log
### 주요 개발 기록
### Effect System 리팩토링
- 카드 효과 처리 구조를 switch 기반에서 Effect 기반으로 전환
- CardData → List<ICardEffect> 구조 도입
- 카드 로직을 Effect 조합으로 정의
- ScriptableObject 기반 카드 데이터 관리

결과

- 카드 추가 시 기존 코드 수정 없이 확장 가능
- 복잡한 카드 효과를 독립적으로 구현 가능
### 상태이상 기반 전투 구조 확립
- Poison / Burn / Vulnerable 상태이상 시스템 구현
- 상태이상 간 시너지 로직 설계 및 적용

결과

- 단순 공격 중심 전투에서 시너지 기반 전투 구조로 전환
### 시스템 구조 분리
- BattleManager / StatusEffectController / RelicManager 역할 분리
- 상태이상 및 유물 로직을 전투 흐름에서 분리

결과

- 결합도 감소 및 유지보수성 향상
### 보상 및 빌드 시스템 구현
- 전투 종료 후 카드 선택 보상 구현
- 유물 보상 구조 및 시작 패시브 선택 시스템 도입

결과

- 플레이어 빌드 방향성 선택 구조 완성

### 유물 시스템 리팩토링
- 캐시 기반 구조 → 이벤트 기반 구조 전환
- ScriptableObject 기반 유물 데이터 관리
- 런타임 effect 조립 구조 도입

결과

- 유물 효과 확장성 확보
- 카드 시스템과 구조 통일

---

### 주요 버그 해결

- 유물 재귀 호출 문제 해결 (OnApplyPoison 루프)
- 조건 검사 시점 오류 수정 (스냅샷 구조 적용)
- PoisonCore / VenomSac 중복 적용 문제 해결
- PressurePoint 턴 제한 버그 수정

## 9. Screenshots / GIFs
### 전투 화면

추후 추가 예정

### 시너지 예시

추후 추가 예정

### 플레이 영상

추후 추가 예정

## 10. Development Convention

### Commit Message Rules
이 프로젝트는 아래 커밋 메시지 규칙을 사용합니다.

- `feat`: 새로운 기능 추가
- `fix`: 버그 수정
- `refactor`: 기능 변화 없이 코드 구조 개선
- `chore`: 프로젝트 설정, 정리, 유지보수 작업
- `docs`: 문서 작성 및 수정
- `test`: 테스트용 코드 및 디버그 작업

형식:
타입: 동사로 시작하는 짧은 설명

예시:
- `feat: add energy-based turn actions`
- `fix: prevent duplicate enemy turn execution`
- `refactor: simplify battle state transition flow`
- `chore: organize Unity project folders`
- `docs: update README with battle system overview`

## 11. Portfolio Goal

이 프로젝트의 최종 목표는 다음 산출물을 남기는 것입니다.

- 플레이 가능한 Unity 빌드
- GitHub 저장소
- README 문서
- 시스템 설명 문서
- 짧은 플레이 영상 (1~2분)
- 주요 시너지 예시 GIF / 캡처

이 프로젝트를 통해
**시너지 기반 시스템을 설계하고 구현할 수 있는 Unity 개발자**라는 점을
명확하게 보여주는 것을 목표로 합니다.
