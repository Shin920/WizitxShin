# Wizit ERP Utility

생산관리 ERP 운영 과정에서 반복적으로 발생하는 장애 및 운영 이슈를
빠르게 해결하기 위해 개발한 **ERP 운영 지원 도구**입니다.

본 프로그램은 SQL Server와 연동하여 운영자가 직접 데이터를 조회하고
필요한 유지보수 작업을 수행할 수 있도록 구현하였습니다.

------------------------------------------------------------------------

# 📌 Overview

운영 중 자주 발생하는 생산관리 ERP 데이터 오류와 SQL Server 블로킹
문제를 신속하게 처리하기 위한 관리자용 유틸리티입니다.

주요 기능은 다음과 같습니다.

-   SQL Server 세션 모니터링
-   Blocking Session 탐지
-   KILL Process 지원
-   공정진행표 발급 취소
-   출고등록 데이터 초기화
-   ERP 운영 데이터 유지보수

------------------------------------------------------------------------

# 🚀 Key Features

## 🔍 Session Monitoring

SQL Server의 현재 접속 세션을 조회하여 운영 상태를 실시간으로
확인합니다.

-   Session 조회
-   로그인 계정 확인
-   Host Name 확인
-   실행 중인 Command 확인
-   Blocking 여부 확인

------------------------------------------------------------------------

## 🚨 Blocking Detection

SQL Server Blocking 관계를 분석하여 문제 세션을 식별합니다.

-   Blocking Session 식별
-   Blocked Session 표시
-   UI 색상 강조
-   Blocking 관계 확인

------------------------------------------------------------------------

## ⚠ Process KILL

관리자 권한에서 Blocking Session을 종료하여 ERP 응답 지연을 해소할 수
있습니다.

> 운영 환경에서는 권한 정책에 따라 기능 비활성화가 필요할 수 있습니다.

기능

-   KILL Process
-   UPDATE / INSERT 작업 보호
-   종료 전 사용자 확인

------------------------------------------------------------------------

## 📄 Process Sheet Cancel

잘못 발급된 공정진행표를 취소할 수 있습니다.

지원 기준

-   생산의뢰번호(PDNO)
-   제조번호(PRNO)

관련 데이터를 일괄 삭제하여 재발급이 가능하도록 지원합니다.

------------------------------------------------------------------------

## 🔄 ERP Data Reset

안산공정 ERP 출고등록 과정에서 누락된 데이터를 초기화하여 재처리할 수
있도록 지원합니다.

기능

-   특정 컬럼 NULL 초기화
-   ERP 재조회 가능
-   운영 데이터 복구 지원

------------------------------------------------------------------------

# 🏗 System Architecture

``` mermaid
flowchart LR

Operator --> Utility

Utility --> SQLServer[(SQL Server)]

SQLServer --> Session
SQLServer --> ERPData

Session --> Monitor
Session --> Kill

ERPData --> ProcessCancel
ERPData --> DataReset
```

------------------------------------------------------------------------

# 📁 Main Functions

  기능                 설명
  -------------------- ----------------------------
  Session Monitoring   SQL Server 세션 조회
  Blocking Detection   Blocking 관계 분석
  KILL Process         Blocking Session 종료
  Process Cancel       공정진행표 취소
  Data Reset           ERP 출고등록 데이터 초기화

------------------------------------------------------------------------

# 🛠 Technology Stack

-   C#
-   .NET Framework WinForms
-   SQL Server
-   ADO.NET

------------------------------------------------------------------------

# ⭐ Technical Highlights

-   SQL Server Session 모니터링
-   Blocking 관계 시각화
-   관리자용 KILL 기능
-   생산 데이터 유지보수 기능
-   ERP 운영 지원 도구
-   UI 기반 운영 관리

------------------------------------------------------------------------

# 🎯 Development Purpose

ERP 운영 중 발생하는 SQL Server 블로킹, 공정진행표 발급 오류 및 출고등록
데이터 누락 문제를 신속하게 해결하기 위해 개발한 관리자용
유틸리티입니다.

운영자가 SQL을 직접 실행하지 않고도 GUI 환경에서 안전하게 유지보수
작업을 수행할 수 있도록 구현하였습니다.
