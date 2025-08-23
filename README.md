# Archer Survival — 기술 소개서

## 📌 프로젝트 개요
- **장르:** 모바일 생존형 액션 RPG
- **개발 목적:** Firebase + Google Play Games Services(GPGS) 연동 경험 및 오브젝트 풀링 기반 최적화
- **컨셉:** 끊임없이 몰려오는 적을 상대하며 성장하는 아케이드형 RPG

---

## 🛠 개발 환경
- **엔진:** Unity 2022.3.15f1  
- **언어:** C#  
- **서비스 연동:**  
  - Firebase (Auth, Firestore)  
  - Google Play Games Services (업적, 랭킹)  
- **플랫폼:** Android (SDK API Level 32 이상)  

---

## ⚔ 시스템 구조 및 구현
### 🔑 핵심 기능
- Firebase 로그인/회원가입 & Firestore 데이터 저장  
- Google Play 업적/리더보드 연동  
- 자동 이동/공격 시스템  
- 경험치 & 레벨업 → 스킬 강화 (FireBall, FF 등)  
- 보스 몬스터 패턴 (돌진, 회전 공격)  
- 오브젝트 풀링 (총알, 몬스터, 아이템)  

### 🏗 아키텍처
- **GlobalValue:** 전역 데이터 관리 (레벨, 경험치, 코인, 아이템 등)  
- **Firebase_Init / LogIn / UserData:** Firebase 초기화 & 데이터 동기화  
- **PlayerCtrl / Bullet_Ctrl / Monster_Ctrl / Boss_Ctrl:** 게임 플레이 핵심 로직  
- **ObjectPool_Mgr:** 오브젝트 풀링 매니저  
- **GameMgr:** 씬 및 UI 관리  

### ⚙ 구현 상세
- 자동 타겟팅 & 발사: ShootCtrl → Bullet_Ctrl  
- EXP 획득 → 레벨업 → 스킬 선택 UI  
- Destroy 대신 SetActive(false) 반환 → 풀링 최적화  
- GPGS 업적: Social.ReportProgress 로 반영  

---

## 🐞 문제 해결 및 개선
- Firebase `google-services.json` 위치 문제 → `Assets/StreamingAssets`로 해결  
- GPGS SDK & compileSdkVersion 충돌 → Gradle 설정 수정  
- 메모리 누수 → 오브젝트 풀링 도입  

---

## 🚀 성과 및 기대 효과
- Firebase 로그인/데이터 저장 안정화  
- GPGS 업적 및 랭킹 연동 완료  
- 저사양에서도 원활한 성능 (FPS 안정화)  
- 상용 서비스 가능 수준까지 개발 완료  

---

## 📅 향후 계획
- 멀티플레이 모드 추가  
- 신규 보스 및 스테이지 확장  
- 업적 세분화, 주간/월간 리더보드  
- 정식 출시 및 이벤트 운영  

---

## 📊 아키텍처 다이어그램 (Mermaid 예시)
```mermaid
flowchart LR
    PlayerCtrl --> ShootCtrl --> Bullet_Ctrl
    PlayerCtrl --> GlobalValue
    Monster_Ctrl --> GlobalValue
    Boss_Ctrl --> GlobalValue
    ObjectPool_Mgr --> Bullet_Ctrl
    GameMgr --> UI
    Firebase_LogIn --> Firebase[Firebase]
    Firebase_UserData --> Firebase

