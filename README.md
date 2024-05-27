# Pium

### 정보

제목: 피움

장르: 캐쥬얼, 시뮬레이션

조작: VR기기

내용: 메타버스에서 만나는 VR 식물 테라피

유저가 그린 꽃,  랜덤으로 자라난 식물,  가구 등을 방에 장식할 수 있으며

친구를 초대해 공간을 공유할 수 있습니다.

---

### 개발

인원: 프로그래머 4인 + 모델링(디자인) 1인

기간: 2개월

사용 프로그램:  Unity3D(C#), Photon, Firebase

---
### 코드 요약

`FurnitureData` 가구의 데이터 타입 저장

`M_ColorPicker` 가구의 색상을 지정 할 수 있는 컬러피커

`M_ColorPickerManager` 지정된 색깔로 가구 색을 변경

`M_GuideQuad` 가구 그림자(설치 가이드)

`M_Inventory` 인벤토리 On/Off, 가구 빼기/넣기, 회전

`M_InventorySlotRotate` 인벤토리 슬롯 회전

`M_InviteRoomManager` 플레이어 방 불러오기

`M_MyRoomInventory` 가구 목록 정렬, 콜라이더 생성

`M_MyRoomManager` 가구 추가, 가구 교체, 방 저장

`M_PlaceManager` 데이터에 맞는 오브젝트 불러오기, 가구 이동, 내려놓기

`M_Tutorial` 방꾸미기(설치, 삭제, 색상 변환, 저장, 초기화, 친구 기능) 튜토리얼
