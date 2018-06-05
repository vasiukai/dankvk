using UnityEngine;
using System.Collections;
using System.Linq;

namespace Bolt.DanKVK
{
    public class PlayerController : Bolt.EntityEventListener<IPlayerState>
    {
        const float MOUSE_SENSEITIVITY = 2f;

        bool forward;
        bool backward;
        bool left;
        bool right;
        bool jump;
        bool aiming;
        bool fire;

        int weapon;

        float yaw;
        float pitch;

        PlayerMotor _motor;

        [SerializeField]
        WeaponBase[] _weapons;

        [SerializeField]
        AudioSource _weaponSfxSource;

      

        void Update()
        {
            PollKeys(true);

            if (entity.isOwner && entity.hasControl && Input.GetKey(KeyCode.L))
            {
                for (int i = 0; i < 100; ++i)
                {
                    BoltNetwork.Instantiate(BoltPrefabs.SceneCube, new Vector3(Random.value * 512, Random.value * 512, Random.value * 512), Quaternion.identity);
                }
            }
        }

        void PollKeys(bool mouse)
        {
            forward = Input.GetKey(KeyCode.W);
            backward = Input.GetKey(KeyCode.S);
            left = Input.GetKey(KeyCode.A);
            right = Input.GetKey(KeyCode.D);
            jump = Input.GetKey(KeyCode.Space);
            aiming = Input.GetMouseButton(1);
            fire = Input.GetMouseButton(0);

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                weapon = 0;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                weapon = 1;
            }

            if (mouse)
            {
                yaw += (Input.GetAxisRaw("Mouse X") * MOUSE_SENSEITIVITY);
                yaw %= 360f;

                pitch += (-Input.GetAxisRaw("Mouse Y") * MOUSE_SENSEITIVITY);
                pitch = Mathf.Clamp(pitch, -85f, +85f);
            }
        }

        public override void Attached()
        {
            if (entity.isOwner)
            {
                state.tokenTest = new TestToken() { Number = 1337 };
            }

            state.AddCallback("tokenTest", () =>
            {
                BoltLog.Info("Received token in .tokenTest property {0}", state.tokenTest);
            });

            state.SetTransforms(state.transform, transform);
            state.SetAnimator(GetComponentInChildren<Animator>());
            state.Animator.SetLayerWeight(0, 1);
            state.Animator.SetLayerWeight(1, 1);

            state.OnFire += OnFire;
            state.AddCallback("weapon", WeaponChanged);
            WeaponChanged();
        }

   

        public override void SimulateOwner()
        {
            if ((BoltNetwork.frame % 5) == 0 && (state.Dead == false))
            {
                state.health = (byte)Mathf.Clamp(state.health + 1, 0, 100);
            }
        }

        public override void SimulateController()
        {
            PollKeys(false);

            IPlayerCommandInput input = PlayerCommand.Create();

            input.forward = forward;
            input.backward = backward;
            input.left = left;
            input.right = right;
            input.jump = jump;


            input.yaw = yaw;
            input.pitch = pitch;



            entity.QueueInput(input);
        }

        public override void ExecuteCommand(Bolt.Command c, bool resetState)
        {
            if (state.Dead)
            {
                return;
            }

            PlayerCommand cmd = (PlayerCommand)c;

            if (resetState)
            {
                _motor.SetState(cmd.Result.position, cmd.Result.velocity, cmd.Result.isGrounded, cmd.Result.jumpFrames);
            }
            else
            {
                var result = _motor.Move(cmd.Input.forward, cmd.Input.backward, cmd.Input.left, cmd.Input.right, cmd.Input.jump, cmd.Input.yaw);

                cmd.Result.position = result.position;
                cmd.Result.velocity = result.velocity;
                cmd.Result.jumpFrames = result.jumpFrames;
                cmd.Result.isGrounded = result.isGrounded;

                if (cmd.IsFirstExecution)
                {
                    AnimatePlayer(cmd);
                    state.pitch = cmd.Input.pitch;
                }
            }
        }

        void AnimatePlayer(PlayerCommand cmd)
        {
            if (cmd.Input.forward ^ cmd.Input.backward)
            {
                state.MoveZ = cmd.Input.forward ? 1 : -1;
            }
            else
            {
                state.MoveZ = 0;
            }
            if (cmd.Input.left ^ cmd.Input.right)
            {
                state.MoveX = cmd.Input.right ? 1 : -1;
            }
            else
            {
                state.MoveX = 0;
            }
            if (_motor.jumpStartedThisFrame)
            {
                state.Jump();
            }
        }
    }
}
