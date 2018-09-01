using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarShip
{
    // 시스템을 만들때 주의 사항
    // 편하자고 만든건데, 이리저리 챙겨야 할 사항이 많으면 많을수록 그건 돌아가는 시스템을 만든게 아니라
    // 만든 사람 & 사용하는 사람 괴롭힐려고 만드는 거나 마찬가지다. 복잡성 관리하세요.

    public abstract class CardInfo
    {
        public enum Type
        {
            Summon,
            Magic,
            Buff
        }

        protected Type type = Type.Magic;
        protected GameManager GM { get { return GameManager.Instance; } }
        protected GameInfo Game { get { return GM.Game; } }
        protected BoardIndicator BoardIndicator {  get { return GM.Board.BoardIndicator; } }
        // 보드 표시를 여기서 접근해서 제어해야 한다. 
        // 누구에게 접근해야 하는가? Board에 바로 접근하는게 맞는듯?

        public virtual string DetectorName
        {
            get
            {
                return "ShipArrival";
            }
        }

        public virtual bool Initialize()
        {
            var Detector = BoardIndicator.AddDetector(DetectorName);

            if (Detector != null)
                Detector.ClearBlock();

            return true;
        }

        public virtual void OnDecision()
        {
            //BoardIndicator.SetEnableDetect(this,true);
        }

        public virtual void OnPreplay()
        {
            BoardIndicator.SetEnableDetect(this, true);
        }

        public virtual void OnHand()
        {
            BoardIndicator.SetEnableDetect(this, false);
        }

        public virtual void OnPlay()
        {
            BoardIndicator.SetEnableDetect(this, false);
            var detected = GetDetectedEnemy();

            foreach (var item in detected)
            {
                item.TakeTrueDamage(1000);
            }
        }

        // Utilities

        public List<BattleObject> GetDetectedEnemy()
        {
            return Game.GetDetectedEnemy(DetectorName, BoardIndicator);
        }
    }
}
