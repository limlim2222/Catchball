# Catchball – 멀티유저 VR 아바타 환경

## 프로젝트 개요
석사 논문 실험을 준비하는 과정에서 개발한 멀티유저 VR 아바타 환경입니다.
HMD, 컨트롤러, Vive Tracker로 추적한 사용자의 움직임을 아바타에 반영하고, Photon을 이용해 다른 사용자의 화면에도 같은 움직임이 보이도록 동기화했습니다.
여러 사용자가 같은 공간에서 공을 주고받고 볼링핀을 쓰러뜨리는 상호작용으로 아바타 추적과 네트워크 동기화를 테스트했습니다.

## 관련 연구
- 석사 학위논문: 가상 현실에서 아바타 외형과 움직임 정확도가 체화감에 미치는 영향 (2025)
- 이 저장소는 논문 실험 준비 단계의 환경이며, 최종 실험 과제(버튼 누르기, 장애물 피해서 걷기)와 설문 기능은 포함되어 있지 않습니다.

## 개발 환경
- Unity 2021.3.3f1, C#
- SteamVR, OpenXR
- HTC Vive Pro, Vive Controller, Vive Tracker (양발)
- Photon PUN 2
- Final IK
- NetMQ (외부 모델 서버와의 통신)

## 주요 기능

### 1. 멀티유저 네트워크 (Photon PUN 2)
- 서버 접속 후 방에 자동으로 입장하고, 방이 없으면 새로 생성합니다.
- 사용자의 머리, 양손, 양발 위치와 회전을 아바타 IK 타겟에 반영해 다른 사용자의 화면에 동기화합니다.
- `photonView.IsMine`으로 내 아바타와 다른 사용자의 아바타를 구분해 처리했습니다.

### 2. 공 네트워크 동기화
- `OnPhotonSerializeView`로 공의 위치, 회전, 속도, 잡힘 상태를 직접 주고받습니다.
- 다른 사용자 화면에서는 받은 값을 보간(Lerp/Slerp)해 공이 부드럽게 움직이도록 했습니다.
- 공을 잡으면 소유권을 가져오고, 잡기와 던지기 상태를 RPC로 모든 사용자에게 전달합니다.
- 공은 방장(Master Client)만 생성하도록 해 사용자 수만큼 공이 중복 생성되는 문제를 막았습니다.

### 3. VR 상호작용
- 컨트롤러 그립 버튼으로 물체를 잡고 놓을 수 있으며, 놓을 때 손의 속도를 적용해 던질 수 있습니다.
- 볼링핀이 모두 쓰러지면 일정 시간 후 자동으로 초기화되고, 버튼으로 바로 초기화할 수도 있습니다.
- 공이 바닥이나 범위 밖으로 떨어지면 원래 위치로 돌아갑니다.
- 컨트롤러 메뉴 버튼으로 일시정지할 수 있습니다.

### 4. 아바타
- 스틱, 남성, 여성, 회색 아바타 프리팹을 네트워크 플레이어로 구성했습니다.
- 아바타의 루트 위치가 HMD 위치를 따라가도록 보정했습니다.
- 아바타가 카메라 범위에서 사라지는 문제를 막기 위해 렌더링 범위(Bounding Box)를 조정했습니다.
- 컨트롤러 버튼으로 사용자 키에 맞게 아바타 크기를 맞추는 캘리브레이션과, 발 Tracker 기반 CCDIK 보정을 적용했습니다.

### 5. 하체 자세 예측 모델 연동
- 연구실에서 개발한 하체 자세 예측 모델(Python)을 사용했습니다.
- Unity에서 HMD와 양손 컨트롤러 데이터를 NetMQ로 모델 서버에 보내고, 받은 관절 자세 값을 아바타에 적용합니다.

## 코드 구조

| 분류 | 스크립트 | 역할 |
|---|---|---|
| 네트워크 | `NetworkManager.cs` | Photon 서버 접속, 방 생성 및 입장 |
| | `NetworkPlayerSpawner.cs` | 플레이어 생성, 방장만 공 생성 |
| | `NetworkPlayer.cs`, `NetworkPlayerTracker.cs` | VR 장비 위치를 아바타 타겟에 동기화 |
| | `ClientSideCamera.cs` | 내 아바타의 카메라 설정 |
| 공·상호작용 | `Ball.cs` | 공 상태 동기화, 보간, 잡기·던지기 RPC |
| | `VRHand.cs` | 물체 잡기, 놓기, 던지기 |
| | `Pin.cs`, `PinManager.cs` | 볼링핀 쓰러짐 확인 및 초기화 |
| | `Return.cs`, `GameManager.cs` | 범위 밖으로 떨어진 물체 초기화 |
| | `MenuButtonHandler.cs`, `External_cam.cs` | 일시정지, 외부 관찰 카메라 |
| 아바타 | `RootBoneUpdate.cs` | 아바타 루트를 HMD 위치에 맞춤 |
| | `AdjustAvatarBoundingBox.cs`, `AvatarFrustumCulling.cs` | 아바타 렌더링 범위 조정 |
| | `AnkleIK.cs` | 다리 관절 IK 테스트 |
| 모델 연동 | `ModelIntegrationManager.cs`, `NetworkClient.cs` | 모델 서버 통신, 예측 결과 적용, 캘리브레이션 |
| | `online/Client.cs`, `offline/PlayCSV.cs` | 실시간 및 저장된 동작 데이터 재생 |

## 씬 구성
- `SampleScene` : 멀티유저 공·볼링 상호작용 환경
- `DummyNetworkScene` : 네트워크 테스트용 더미 씬
- `IKTestScene` : IK 테스트

## 참고
- 하체 자세 예측 모델은 이 저장소에 포함되어 있지 않으며, 실행하려면 별도의 모델 서버(`tcp://127.0.0.1:3550`)가 필요합니다.
- Photon 실행을 위해 Photon App ID 설정이 필요합니다.
