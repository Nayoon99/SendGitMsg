# SendGitMsg 📨  
하루 1회 Git 커밋 여부를 확인하고 기록하는 WPF 자동화 연습 프로젝트

---

## 📌 프로젝트 목적

- 하루에 **한 번만**
- **전날 Git 커밋 여부 및 커밋 수를 확인**
- 결과를 **로컬 DB(SQLite)에 저장**
- 커밋 체크 결과를 **WPF UI(DataGrid)** 로 확인
- SMS / Slack 메시지 전송 기능

---

## 🛠 기술 스택

- Visual Studio .NET 8.0
- git version 2.52.0.windows.1
- Language: **C#**
- UI: **WPF**
- Architecture: **MVVM 패턴**
- Database: **SQLite**
- ORM: 직접 SQL 사용
- Package:
  - `System.Data.SQLite`

---

## 📂 프로젝트 구조

```text
SendGitMsg
 ├─ View
 │   └─ MainWindow.xaml
 │
 ├─ ViewModel
 │   └─ MainViewModel.cs
 │
 ├─ Model
 │   └─ CommitRecord.cs
 │
 ├─ Repository
 │   └─ CommitLogRepository.cs
 │
 ├─ SQLite
 │   └─ DatabaseInitializer.cs
 │
 └─ App.xaml.cs

 --

 cf. ViewModel => "언제 보내야 하는지"만 판단
     Service => "어떻게 보내는지"만 담당


## 설치 패키지
- SQLite : System.Data.SQLite
- JSON 처리: Newtonsoft.Json
- SMS 전송: CoolSms

## 구조
config.json   ← 민감 정보 (API Key, 전화번호)
Config.cs     ← config.json 매핑용 클래스
SmsService    ← Config 주입받아서 문자 전송
MainViewModel ← SmsService 사용