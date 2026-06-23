# ERP Blocking Monitor

위지트 생산관리부의 ERP 사용 중 발생하는 반복적인 지연 및 버퍼링 현상에 대한 원인 파악을 지원하기 위해 개발한 SQL Server 모니터링 도구입니다.

---

## 📌 Key Features

### Session Monitoring

sp_who2 결과를 기반으로 현재 SQL Server 접속 세션 정보를 조회합니다.

### Employee Mapping

PC HostName 끝 5자리 사번 정보를 기준으로 사원 마스터(HI230)와 연동하여 사용자명을 표시합니다.

### Blocking Session Detection

BlkBy 값을 분석하여 블로킹 관계를 식별합니다.

### Visual Highlight

다른 세션을 차단하고 있는 원인 세션은 Grid에서 빨간색으로 표시됩니다.

### Lightweight Operation

SQL Server 2000 환경을 고려하여 최소한의 조회 기능만 제공합니다.

### Session Kill

DML(Data Manipulation Language)를 제외한 작업 세션을 종료할 수 있는 기능을 제공합니다.

---

## 🖥️ Example

| SPID | 사용자 | PC명 | 상태 |
|------|--------|------|------|
| 62 | 김성광 | DESKTOP-JCCAH | 🔴 Blocking |
| 71 | 홍길동 | DESKTOP-ABC01 | Waiting |

---

## 🚫 Not Supported

- Session Kill
- SQL 실행
- 데이터 수정
- 관리자 기능

세션 강제 종료(KILL)는 데이터 손상 및 업무 영향 가능성이 있으므로 제한된 기능으로만 수행합니다.
