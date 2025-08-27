# Archer Survival — 기술 소개서

> Unity 기반 모바일 생존형 액션 RPG  
> Firebase & Google Play Games Services(GPGS) 연동 · 오브젝트 풀링(Object Pooling) 최적화 · 자동 전투 · 레벨업 & 스킬 성장 루프

---

## 목차
- [개요](#개요)
- [핵심 기능](#핵심-기능)
- [개발 환경](#개발-환경)
- [아키텍처](#아키텍처)
- [폴더 구조](#폴더-구조)
- [구현 상세](#구현-상세)
  - [1) 계정/데이터 (Firebase)](#1-계정데이터-firebase)
  - [2) 업적/랭킹 (GPGS)](#2-업적랭킹-gpgs)
  - [3) 전투/자동타겟팅/총알](#3-전투자동타겟팅총알)
  - [4) 오브젝트 풀링](#4-오브젝트-풀링)
  - [5) 레벨업/스킬 시스템](#5-레벨업스킬-시스템)
- [최적화 포인트](#최적화-포인트)
- [문제 해결](#문제-해결)
- [문제 해결 사례](#문제-해결-사례-troubleshooting-cases)
- [빌드 & 배포](#빌드--배포)
- [향후 계획](#향후-계획)
- [라이선스](#라이선스)

---

## 개요
- **장르:** 모바일 생존형 액션 RPG  
- **핵심 목표:** Firebase + GPGS로 계정/데이터/업적 안정 연동, 오브젝트 풀링으로 저사양 기기에서도 프레임 안정화, 자동 전투 + 레벨업 + 스킬 선택으로 빠른 성장감 제공  

<p align="center">
  <img src="docs/assets/hero.png" width="720" alt="게임 대표 이미지">
</p>

---

## 핵심 기능
- **계정 시스템:** Firebase Auth(이메일/비밀번호), Google Play Games 로그인 지원  
- **데이터 동기화:** Firestore에 유저별 코인/아이템/업적 저장 및 로드  
- **전투/AI:** 자동 타겟팅 & 발사, 보스 패턴(돌진/360도 등)  
- **성장 루프:** EXP → 레벨업 → 스킬 선택(예: `FireBall`, `FireBall_FF`)  
- **최적화:** 총알/몬스터/EXP/코인에 오브젝트 풀 적용으로 GC/프레임 드랍 최소화  

---

## 개발 환경
- **엔진/언어:** Unity `2022.3.15f1`, C#  
- **서비스 연동:** Firebase(`Auth`, `Firestore`), Google Play Games Services  
- **플랫폼:** Android(`minSdk` 프로젝트 설정 기준, `compileSdk 34` 대응)  

---

## 아키텍처
- **Gameplay:** `PlayerCtrl`, `ShootCtrl`, `Bullet_Ctrl`, `Monster_Ctrl`, `Boss_Ctrl`, `ObjectPool_Mgr`, `GameMgr`, `GlobalValue`  
- **Backend:** `Firebase_Init` → `Firebase_LogIn` → `Firebase_UserData`  
- **GlobalValue:** 전역 상태 관리 + Firestore 동기화  

<!-- Mermaid 다이어그램 -->
```mermaid
flowchart LR
  %% ======= Gameplay (Client) =======
  subgraph G[Gameplay_Client]
    PlayerCtrl
    ShootCtrl
    Bullet_Ctrl
    Monster_Ctrl
    Boss_Ctrl
    ObjectPool_Mgr
    GameMgr
    GlobalValue
    UI
  end

  %% ======= Backend (Cloud) =======
  subgraph B[Backend_Cloud]
    Firebase_Init
    Firebase_LogIn
    Firebase_UserData
    Firestore[(Firestore)]
    Auth[(Auth)]
  end

  %% ======= External =======
  subgraph S[External_Services]
    GPGS[Google_Play_Games]
  end

  %% Gameplay wiring
  PlayerCtrl --> ShootCtrl --> Bullet_Ctrl
  ObjectPool_Mgr --> Bullet_Ctrl
  ObjectPool_Mgr --> Monster_Ctrl
  PlayerCtrl --> GlobalValue
  Monster_Ctrl --> GlobalValue
  Boss_Ctrl --> GlobalValue
  GameMgr --> UI
  GameMgr --> GlobalValue

  %% Backend wiring
  Firebase_Init --> Auth
  Firebase_Init --> Firestore
  Firebase_LogIn --> Auth
  Firebase_UserData <--> Firestore
  GlobalValue <--> Firebase_UserData

  %% External
  GameMgr --> GPGS
  GPGS ---|Achievements/Leaderboards| GameMgr

```

## 폴더 구조
- `Assets/01.Scripts/Managers`: `GameMgr`, `ObjectPool_Mgr`  
- `Assets/01.Scripts/Gameplay`: `PlayerCtrl`, `ShootCtrl`, `Bullet_Ctrl`, `Monster_Ctrl`, `Boss_Ctrl`  
- `Assets/01.Scripts/Systems`: `GlobalValue`, UI 관리  
- `Assets/01.Scripts/Backend`: `Firebase_Init`, `Firebase_LogIn`, `Firebase_UserData`  
- `StreamingAssets`: `google-services.json`  
- `docs/assets`: 문서용 이미지  

---

## 구현 상세

### 1) 계정/데이터 (Firebase)
- **인증:** Firebase Auth로 이메일/비밀번호 로그인, GPGS 로그인 연동  
- **저장/로드:** Firestore에 코인, 아이템, 업적 저장 → 앱 시작 시 로드  
- **흐름:** Init → Login → UserData 로드 → 진행 중 데이터 저장  

- **Firebase Auth 로그인 코드 예시**  
  📄 [FirebaseAuth_Mgr.cs](https://github.com/hso121025/Archer-Survival/blob/main/Assets/02.Scripts/FirebaseAuth_Mgr.cs)
  
<p align="center">
  <img src="docs/assets/Firebase.png" width="720" alt="Firebase 이미지">
</p>

<p align="center">
  <img src="docs/assets/firebase.png" width="720" alt="firebase 이미지">
</p>

---

### 2) 업적/랭킹 (GPGS)
- **초기화:** `PlayGamesPlatform.Activate()`로 GPGS 인증  
- **업적 달성:** `Social.ReportProgress(achievementId, 100.0, ...)` 호출  
- **리더보드:** `Social.ShowLeaderboardUI()`로 표시  

- **업적 달성 시도 코드 (GPGS 연동 포인트)**  
  📄 [FirebaseAuth_Mgr.cs](https://github.com/hso121025/Archer-Survival/blob/main/Assets/02.Scripts/FirebaseAuth_Mgr.cs)
  
<p align="center">
  <img src="docs/assets/업적.png" width="720" alt="업적 이미지">
</p>

<p align="center">
  <img src="docs/assets/리더보드.png" width="720" alt="리더보드 이미지">
</p>

---

### 3) 전투/자동타겟팅/총알
- **타겟팅:** `ShootCtrl` → 가까운 몬스터 탐색, 방향 계산 
- **투사체:** `Bullet_Ctrl` → 전진, 수명, 충돌 처리 후 풀 반환  
- **적 처리:** `Monster_Ctrl` → 추적, 피격, 사망 시 아이템 드랍  

- **ShootCtrl.FindNearestEnemy()**  
  📄 [ShootCtrl.cs](https://github.com/hso121025/Archer-Survival/blob/main/Assets/02.Scripts/ShootCtrl.cs)
- **총알 발사 코드 (FirePos 사용)**  
  📄 [ShootCtrl.cs](https://github.com/hso121025/Archer-Survival/blob/main/Assets/02.Scripts/ShootCtrl.cs)

---

### 4) 오브젝트 풀링
- **목표:** `Destroy()` 최소화 → GC/프레임 드랍 제거  
- **매니저:** `ObjectPool_Mgr` → `Get(key)`, `Return(obj)` 관리  
- **적용 범위:** 총알, 몬스터, EXP/코인 등  

- **풀링 매니저 (`Get` / `Return`)**  
  📄 [BulletPool_Mgr.cs](https://github.com/hso121025/Archer-Survival/blob/main/Assets/02.Scripts/BulletPool_Mgr.cs)  
  📄 [EXPPool_Mgr.cs](https://github.com/hso121025/Archer-Survival/blob/main/Assets/02.Scripts/EXPPool_Mgr.cs)

---

### 5) 레벨업/스킬 시스템
- **EXP 누적:** 몬스터 처치 → EXP 아이템 드랍 → 획득  
- **레벨업:** 임계치 도달 시 레벨 증가, 남은 EXP 이월  
- **스킬 선택:** UI 표시 → 선택 즉시 반영  
- **예시 스킬:** `FireBall_FF`: 직전 발사 수만큼 추가 발사  

- **GlobalValue.AddExp() / 성장 루프**  
  📄 [GlobalValue.cs](https://github.com/hso121025/Archer-Survival/blob/main/Assets/02.Scripts/GlobalValue.cs)  
  📄 [GameMgr.cs](https://github.com/hso121025/Archer-Survival/blob/main/Assets/02.Scripts/GameMgr.cs)
- **FireBall_FF 추가 발사 처리**  
  📄 [Skill_Mgr.cs](https://github.com/hso121025/Archer-Survival/blob/main/Assets/02.Scripts/Skill_Mgr.cs)
  
<p align="center">
  <img src="docs/assets/레벨업.png" width="720" alt="레벨업 UI">
</p>

---

## 최적화 포인트
- **Destroy 최소화:** 풀링 + `OnEnable` 초기화  
- **Update 최적화:** 탐색 주기/반경 조정, 캐싱, 쿨다운 사용  
- **Physics:** 충돌 매트릭스 최적화  
- **UI:** 값 변경 이벤트 기반 갱신  

---

## 빌드 & 배포
- **환경:** Unity 2022.3.15f1 · Android SDK API 32+  
- **절차:**  
  - APK/AAB 빌드  
  - Keystore 서명  
  - Google Play Console 업로드(버전 코드 증가)  
  - GPGS 업적/리더보드 ID 연결, SHA-1 등록  

<p align="center">
  <img src="docs/assets/Project Setting.png" width="720" alt="Project Setting">
</p>

<p align="center">
  <img src="docs/assets/gpgs.png" width="720" alt="GPGS">
</p>

---

## 문제 해결
- **Firebase 초기화 실패:** `google-services.json` 경로/포맷 수정  
- **GPGS/Gradle 충돌:** `compileSdkVersion` 조정, gradle.properties 수정  
- **프레임 드랍:** `Destroy()` 남발 → 풀링 전환  
- **AI 타겟팅 문제:** 캐싱 및 널 가드 처리  

---

## 문제 해결 사례 (Troubleshooting Cases)

### 1) Firebase 초기화 실패 (`google-services.json` 누락)
- **문제:**  
  안드로이드 빌드 시 `Unable to load Firebase app options...` 오류 발생.  
  `StreamingAssets/google-services.json` 경로를 Unity가 인식하지 못해 초기화 실패.  
- **해결 노력:**  
  - `Assets/StreamingAssets` 폴더 생성 후 `google-services.json` 직접 배치  
  - 에디터 전용 `Editor` 스크립트 작성 → 빌드 시 자동 복사되도록 개선  
- **결과:**  
  Firebase Auth/Firestore 초기화 성공, 안드로이드 기기에서도 정상 로그인 가능  

---

### 2) GPGS & Gradle 충돌 (`compileSdkVersion` 불일치)
- **문제:**  
  GPGS v0.11.01 추가 후 Gradle 빌드 시  
checkReleaseAarMetadata
compileSdkVersion is lower than required
오류 발생.  
- **해결 노력:**  
- Unity `2022.3.15f1` 환경에서 `gradle.properties` 수정  
- `android.suppressUnsupportedCompileSdk=34` 추가  
- Android SDK API Level 32 → 34로 상향 조정  
- **결과:**  
GPGS 로그인/업적/리더보드 기능 정상 동작  

---

### 3) 프레임 드랍 (총알/몬스터 `Destroy()` 남발)
- **문제:**  
몬스터 다수 등장 시 `Destroy()` 반복 호출로 GC 부하 → 프레임 급격히 하락  
- **해결 노력:**  
- 총알, 몬스터, EXP, 아이템 전부 오브젝트 풀링으로 전환  
- `OnEnable/OnDisable`에서 상태 초기화 처리  
- **결과:**  
FPS 드랍 현상 개선, 500마리 동시 스폰 환경에서도 60FPS 유지  

---

### 4) Firebase 로그인 예외 처리
- **문제:**  
이미 가입된 이메일 재가입 시 `AggregateException` 발생 → 앱 강제 종료  
- **해결 노력:**  
- `task.IsFaulted` 시 예외 메시지 출력 + UI 안내  
- 로그인/회원가입 버튼 로직 분리  
- **결과:**  
사용자 경험 개선, 중복 계정 문제 예방

---

### 5) AI 타겟팅 문제
- **문제:**  
- `ShootCtrl`이 `Update()`마다 `GameObject.FindGameObjectsWithTag("Monster")`를 호출 → 몬스터 수가 많아지면 프레임 드랍 발생  
- 몬스터가 풀로 반환된 직후에도 참조를 유지 → `NullReferenceException` 오류 다수 발생  
- **해결 노력:**  
- 탐색 최적화: 일정 주기(0.2초 간격)마다만 탐색, `Physics.OverlapSphere`로 범위 내 몬스터만 확인  
- 캐싱 처리: 유효한 타겟이 있으면 유지, 죽거나 멀어졌을 때만 새 탐색  
- 널 가드: `if (targetEnemy == null || !targetEnemy.activeSelf)` 조건 추가  
- **결과:**  
- 다수 몬스터 스폰 환경에서도 FPS 안정화  
- `NullReferenceException` 제거, 자동 타겟팅이 안정적으로 동작  

---

## 향후 계획
- **멀티플레이 지원** (실시간/협동 모드)  
- **콘텐츠 확장**: 신규 보스/스테이지, 이벤트 시스템  
- **경쟁 강화**: 주간/월간 랭킹, 업적 세분화  
- **데이터 품질**: 클라우드 세이브 충돌 해결/머지  
- **UX 개선**: 접근성 옵션(진동, 색각 보정) 추가  

---

## 라이선스
- **MIT License**: 자유로운 사용/수정/배포 가능 (자세한 내용은 LICENSE 파일 참조)
