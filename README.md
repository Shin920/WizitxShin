ERP Blocking Monitor

위지트 생산관리부의 ERP 사용 중 발생하는 반복적인 지연 및 버퍼링 현상에 대한 원인 파악을 지원하기 위해 개발한 SQL Server 모니터링 도구입니다.

Key Features
Session Monitoring

sp_who2 결과를 기반으로 현재 SQL Server 접속 세션 정보를 조회합니다.

Employee Mapping

PC HostName 끝 5자리 사번 정보를 기준으로 사원 마스터(HI230)와 연동하여 사용자명을 표시합니다.

Blocking Session Detection

BlkBy 값을 분석하여 블로킹 관계를 식별합니다.

Visual Highlight

다른 세션을 차단하고 있는 원인 세션은 Grid에서 빨간색으로 표시됩니다.

이를 통해 사용자는 문제를 유발하는 세션을 직관적으로 확인할 수 있습니다.

Lightweight Operation

SQL Server 2000 환경을 고려하여 최소한의 조회 기능만 제공하며, 별도 에이전트 설치 없이 동작합니다.

Scope

본 프로그램은 원인 확인을 위한 모니터링 도구이며 데이터베이스 관리 기능은 제공하지 않습니다.

Not Supported
Session Kill
SQL 실행
데이터 수정
관리자 기능

세션 강제 종료(KILL)는 데이터 손상 및 업무 영향 가능성이 있으므로 전산실 관리자 권한으로만 수행합니다.
