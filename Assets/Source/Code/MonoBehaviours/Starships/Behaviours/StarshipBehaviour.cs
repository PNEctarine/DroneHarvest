using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

namespace Source.Code.MonoBehaviours.Starships.Behaviours
{
    public class StarshipBehaviour : MonoBehaviour
    {
        [field: SerializeField] public NavMeshAgent NavMeshAgent { get; private set; }
        [field: SerializeField] public Transform StarshipViewTransform { get; private set; }
        
        [Space(10)]
        [SerializeField] private LineRenderer _pathLine;
        
        private Vector3 _targetPosition;
        private bool _isFlying;
        private bool _isReturning;

        public int LaunchingPadIndex { get; private set; }
        
        private Vector3 _launchingPadPosition;

        public void Init(Vector3 launchingPadPosition, int launchingPadIndex)
        {
            _launchingPadPosition = launchingPadPosition;
            LaunchingPadIndex = launchingPadIndex;
        }

        public void SetPath(Vector3 target, bool shouldReturn)
        {
            _targetPosition = target;

            if (!NavMeshAgent.enabled)
            {
                NavMeshAgent.enabled = true;
            }
            
            NavMeshAgent.SetDestination(_targetPosition);

            if (!_isFlying)
                TakeOff();
        }
        
        public void Landing()
        {
            float targetLocalY = _launchingPadPosition.y - transform.position.y;
            StarshipViewTransform.DOLocalMoveY(targetLocalY, 1f).SetEase(Ease.InOutCubic).OnComplete(() =>
            {
                NavMeshAgent.enabled = false;
                
                _isFlying = false;
                _isReturning = false;
                
            }).SetAutoKill(true);
        }
        
        public void TakeOff()
        {
            StarshipViewTransform.DOLocalMoveY(15, 1f).SetEase(Ease.InOutCubic).OnComplete(() =>
            {
                NavMeshAgent.enabled = true;
                
                _isFlying = true;
                
            }).SetAutoKill(true);
        }

        private Vector3[] lastPathPoints;

        public void DrawPath(bool show, float yOffset = 0.5f)
        {
            if (_pathLine == null)
                return;

            if (!show)
            {
                _pathLine.enabled = false;
                return;
            }

            NavMeshAgent agent = NavMeshAgent;
            
            if (agent == null)
                return;

            Vector3[] points = null;

            if (agent.path.corners.Length > 0)
            {
                Vector3[] corners = agent.path.corners;
                
                points = new Vector3[corners.Length + 1];
                points[0] = agent.transform.position + Vector3.up * yOffset;
                
                for (int i = 0; i < corners.Length; i++)
                {
                    points[i + 1] = corners[i] + Vector3.up * yOffset;
                }

                lastPathPoints = points;
            }
            
            else if (lastPathPoints != null)
            {
                points = lastPathPoints;
            }

            if (points == null)
            {
                _pathLine.enabled = false;
                return;
            }

            points[0] = agent.transform.position + Vector3.up * yOffset;
            
            _pathLine.enabled = true;
            _pathLine.positionCount = points.Length;
            _pathLine.SetPositions(points);
        }
    }
}
