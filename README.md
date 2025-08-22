# 7조 TeamSurvival

<img width="1407" height="790" alt="제목 없음" src="https://github.com/user-attachments/assets/29ae5d9f-56f2-431e-8588-769285e17a81" />

## 프로젝트 개요
래퍼런스 : 3D 서바이벌 게임
<br />
개발 기간 : 2025.08.14 ~ 2025.08.22
<br />
개발 엔진 : Unity
<br />
플랫폼 : Window

## 팀원 정보
| 이름 | 역할 | 담당 업무 |
|------|------|------|
| 강형욱 | 팀장 | 건축 시스템, 제작 시스템 구현 |
| 김유경 | 팀원 | 아이템 데이터, 오브젝트 구현 |
| 김준형 | 팀원 | 날씨 및 시간,  동물 스폰 구현 |
| 김혜현 | 팀원 | NPC 및 대화, 사운드 구현 |
| 성준우 | 팀원 | 스탯UI, 장착 아이템 구현 |
| 김재경 | 튜터 | SA 피드백, 프로젝트 기술 질의 응답 |

### 🧪 테스트 방법
```markdown
## 테스트 방법

- `ESC` 키로 옵션 UI를 확인 할수있습니다.
- `Tab` 키로 인벤토리 UI를 확인 할수있습니다.
- `C` 키로 제작 UI를 확인 할수있습니다.
- `E` 키로 자원 수집 상호작용 할수있습니다.
- `B` 키로 건축 모드로 진입 할수있습니다.
```

## 주요기능
| 기능 | 설명 |
|------|------|
| 🪓 자원 수집 | 나무, 돌, 음식 등 생존에 필요한 자원을 채집 |
| ❤️ 상태 관리 | 체력, 허기, 갈증 등 플레이어의 상태를 실시간으로 관리 |
| 🏗️ 건축 기능 | 자원을 활용해 거주지 및 방어 시설을 건설 |
| 🧭 생존 시스템 | 날씨, 시간, 자원 고갈 등 다양한 생존 요소 반영 |
| ⚔️ 전투 시스템 | 야생 동물 및 적 NPC와의 전투 구현 |
| 🔄 자원 리스폰 | 일정 시간 후 자원이 다시 생성되어 지속적인 플레이 가능 |
| 🤝 NPC 상호작용 | 퀘스트, 거래, 전투 등 다양한 방식으로 NPC와 상호작용 |
| 🌦️ 날씨 및 시간 | 실시간 날씨 변화와 밤낮 구분으로 전략적 플레이 유도 |
| 🎵 사운드 및 음악 | 몰입감을 높이는 배경음악과 효과음 구현 |

## 프로젝트 구조
```plaintext
Assets/
├── Scenes/                     # 게임 씬 파일들
├── Scripts/                    # C# 스크립트
│   ├── Player/                 # Player의 시스템 관리
│   ├── AI/                     # AI 관련 스크립트
│   ├── Building System/        # 건축 기능 관리
│   ├── Core/                   # 게임핵심 기능 관리 ex ) ManagerScript
│   └── Entity/                 # Entity 상태 관련
│   └── Item/                   # ItemObject 관리 스크립트
│   ├── Composition/            # 제작 관련 스크립트
│   └── UI/         			# 기능별 UI 스크립트
│   └── ScriptableObject/       # Scriptable base script
│   └── Weather/         		# 기상, 시간 변화 스크립트
├── Prefabs/                    # 프리팹 오브젝트
├── Fonts/                      # Font 리소스
├── Materials/                  # 머티리얼 리소스
├── Audio/                      # 사운드 파일
├── Animations/                 # 애니메이션 클립 및 컨트롤러
└── UI/                         # UI 이미지, 캔버스, 버튼 등
```

 ## 기획
<img width="1542" height="707" alt="프레임" src="https://github.com/user-attachments/assets/97e04d5a-6210-428f-8bb4-fae6433b9600" />

## 플레이 사진
<img width="1410" height="800" alt="제목 없음1" src="https://github.com/user-attachments/assets/7f823663-ae9a-41ca-882d-eb274ccf47ff" />


<img width="1199" height="665" alt="스크린샷 2025-08-22 101514" src="https://github.com/user-attachments/assets/46192d0b-fc4d-446a-b177-825ba40c3b04" />
