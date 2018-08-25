using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReturnToEarth
{
    /// <summary>
    /// 자체적으로 시작하는 QNA 
    /// 무엇을 할 것인가? 게임을 만듭니다
    /// 왜 방송을 합니까? 여러 방식에 대한 시도 중입니다. 
    /// 주로 한 분야에 집중하는데 있어서 본인이 어려움을 겪고 있는데
    /// 이에 대한 해결책으로 많은 사람들이 참여 할 수 있는 플렛폼에서
    /// 시작한다면 의미가 있다고 생각해서 시작했습니다.
    /// 
    /// 무엇을 만들 것인가? : 유니티를 사용해서 만듭니다. 
    /// 방송은 언제 이루어 질 것인가? : 주말에 여유 시간을 내서 만듭니다.
    /// 어떻게 만들 것인가
    /// 그 범위는 어느 수준인가? : 장르는 지인과 이야기 한 시점에서 탄막을 만들
    /// 예정입니다. 단순히 게임에 큰 취향을 넣지 않는다면 빠르게 만들 주제이고
    /// 그 완성도를 인디 수준이 아니라 단순히 프로토 타입 정도 선에서 
    /// 마무리 지을 예정입니다.
    /// 
    /// 게임의 완성도에서 (프로토 타입 -> 인디 수준 -> A..AAA) 라 한다면 진짜
    /// 가장 낮은 단계라고 보시면 됩니다.
    /// 
    /// 이미 어느정도 친 코드가 있는데, 진행 정도에 따라 다 다시 수정할 코드들입니다.
    /// 
    /// 유니티는 게임 업계 와서 1.5년 정도 다루었고, 지금은 언리얼을 하고 있습니다.
    /// 개인 프로젝트를 하는데 사실 아직 지금 집에 PC가 없이 노트북으로 하고 있는 거라
    /// 언리얼 보다는 유니티 복습이 좋을거라고 판단합니다.
    /// 
    /// 코드를 올릴 장소는 지금 프로젝트는 공개가 아닌 비 공개로 하려고 합니다.
    /// 그래서 아틀라스의 빗버킷을 활용합니다.
    /// 형상관리 프로그램은 소스트리를 사용 합니다.
    /// 
    /// 나중에 여기서 적은 글들을 이리저리 소계글에 옮기거나 하겠죠
    /// 
    /// 해당 프로젝트는 결과적으로 안드로이드 마켓에 올리는 걸 목표로 합니다. 
    /// 가격은 받지 않을 자기 만족을 위한 목표로 잡았습니다. (나도 안드로이드 마켓에 올릴줄 안다?)
    /// 
    /// 대화는 채팅으로 요청을 하면 제가 말을 하겠는데, 굳이 저도 대화가 있지 않으면 말은 하지 않을려고 합니다.
    /// 
    /// 일단 금일 이루어지는 상황은 모두 테스트를 위하고 있습니다.
    /// 
    /// 아직 정확히 게임의 틀이 상정되지 않았습니다.
    /// 무엇을 구현할 것인가에 대한 명확한 목표 없이는 저는 코드를 잘 치지 못하는 편입니다.
    /// 랜덤하게 스폰되는 유닛들
    /// 그 유닛들을 클릭할 수 있다.
    /// 게임 루프 : [
    /// 클릭하여 드레그 행위를 할 때
    ///     상대방이 존재하면 전투행위를 하고
    ///     상대방이 존재하지 않는다면 이동이 된다.
    ///     한번의 행위는 한턴이고, 턴 목적은 상대방이 모두 죽을 경우 게임 종료가 된다.
    /// ]
    /// 위 분기를 게임 루프라 정의하고, 해당 게임 루프가 완료 되었을 경우 
    /// 사운드 추가 + 에셋을 이용하여 그래픽 보강을 하고
    /// 해당 게임 루프를 재 반복할 수 있는 구조를 작성 한뒤 
    /// 에셋 스토어에 출시한다.
    /// 
    /// 기간은 9월 말이며, 시간상으로 10일 정도 남짓한다. 충분히 완성할 수 있는 양이라 판단하고
    /// 일단은 만드는 행위에 익숙하기 위한 연습도구로서 사용하고자 한다.
    /// </summary>
    public class GameController : MonoBehaviour
    {
        [SerializeField]
        private Vector3 uniformScale;
        [SerializeField]
        private Vector3 uniformCenter;

        // Use this for initialization

        [SerializeField]
        private BoardController boardController;

        [SerializeField]
        private ActorController actorController;

        // Singleton Implementation.
        private static GameController instance = null;
        public static GameController Instance
        {
            get
            {
                if (instance == null)
                    instance = (GameController)FindObjectOfType(typeof(GameController));
                return instance;
            }
        }

        public Vector3 UniformScale
        {
            get
            {
                return uniformScale;
            }
        }

        public Vector3 UniformCenter
        {
            get
            {
                return uniformCenter;
            }
        }

        private void Awake()
        {
            GameDefine.Result results;

            results = boardController.Initialize(uniformCenter, UniformScale);
            results = actorController.Initialize(boardController, uniformCenter, UniformScale);

            // 개략적인 게임 흐름을 그린다면.

            // 레벨디자인 제어는 게임 컨트롤러에서 한다.
            // 레벨 디자인에 맞는 데이터를 가져오고
            // 해당 데이터를 엑터 컨트롤러에서 생성하도록 한다.
            // 레벨 디자인이 있는 셈 치고

            actorController.GenerateUnit(0, 0, Unit.Team.Enemy);
            actorController.GenerateUnit(1, 1, Unit.Team.Enemy);
            actorController.GenerateUnit(2, 2, Unit.Team.Enemy);
            actorController.GenerateUnit(3, 2, Unit.Team.Enemy);
            actorController.GenerateUnit(2, 4, Unit.Team.Enemy);

            actorController.GenerateUnit(2, 1, Unit.Team.Friendly);
            actorController.GenerateUnit(3, 4, Unit.Team.Friendly);
            actorController.GenerateUnit(4, 2, Unit.Team.Friendly);
            actorController.GenerateUnit(5, 4, Unit.Team.Friendly);
            actorController.GenerateUnit(4, 0, Unit.Team.Friendly);

            Debug.Log(results);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
