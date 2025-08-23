# Archer Survival — 기술 소개서 (확장판)

<p align="center">
  <img src="docs/assets/hero.png" alt="게임 스크린샷" width="720">
</p>

<p align="center">
  <b>Unity 기반 생존형 액션 RPG</b> — Firebase & Google Play Games Services 연동, 오브젝트 풀링 기반 최적화
</p>

---

## 목차
- [개요](#개요)
- [핵심 기능](#핵심-기능)
- [개발 환경](#개발-환경)
- [시스템 구조](#시스템-구조)
  - [아키텍처 다이어그램](#아키텍처-다이어그램)
  - [폴더 구조](#폴더-구조)
- [구현 상세](#구현-상세)
  - [1) 계정/데이터 (Firebase)](#1-계정데이터-firebase)
  - [2) 업적/랭킹 (GPGS)](#2-업적랭킹-gpgs)
  - [3) 전투/자동타겟팅/총알](#3-전투자동타겟팅총알)
  - [4) 오브젝트 풀링](#4-오브젝트-풀링)
  - [5) 레벨업/스킬 시스템](#5-레벨업스킬-시스템)
- [최적화 포인트](#최적화-포인트)
- [문제 해결(트러블슈팅)](#문제-해결트러블슈팅)
- [빌드 & 배포](#빌드--배포)
- [향후 계획](#향후-계획)
- [라이선스](#라이선스)

---

## 개요
- **장르:** 모바일 생존형 액션 RPG  
- **목표:**  
  - Firebase + GPGS로 계정/데이터/업적 연동  
  - 오브젝트 풀링(Object Pooling)으로 성능 안정화  
  - 자동 전투 + 레벨업 + 스킬 선택의 성장감  
- **핵심 키워드:** Unity 2022.3, Firebase Auth/Firestore, Google Play Games, Object Pool, Cinemachine

---

## 핵심 기능
- **로그인/회원가입**: Firebase Auth (이메일/비번, GPGS 로그인 지원)  
- **데이터 동기화**: Firestore에 코인/아이템/업적 저장  
- **전투 시스템**: 자동 타겟팅 & 발사, 보스 패턴(돌진/360도 등)  
- **성장 시스템**: EXP → 레벨업 → 스킬 선택 (예: FireBall, FF 추가발사)  
- **최적화**: 총알/몬스터/EXP/코인 풀링으로 GC/프레임 드랍 최소화

---

## 개발 환경
- **Unity**: 2022.3.15f1  
- **언어**: C#  
- **SDK**: Android SDK API 32+  
- **서비스**: Firebase(Auth/Firestore), Google Play Games Services(업적/랭킹)

---

## 시스템 구조

### 아키텍처 다이어그램
```mermaid
flowchart LR
  subgraph Gameplay
    PlayerCtrl --> ShootCtrl --> Bullet_Ctrl
    PlayerCtrl --> GlobalValue
    Monster_Ctrl --> GlobalValue
    Boss_Ctrl --> GlobalValue
    ObjectPool_Mgr --> Bullet_Ctrl
    GameMgr --> UI
  end

  subgraph Backend
    Firebase_Init --> Firebase[(Firebase)]
    Firebase_LogIn --> Firebase
    Firebase_UserData --> Firebase
  end

  UI --> GameMgr
  GlobalValue <--> Firebase_UserData
