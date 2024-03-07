using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TheAshBot.Assets.TextAndVoiceGenerationToolkit.OpenAI.Examples
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private float timeTillNextFrame = 0.25f;
        [SerializeField] private float moveSpeed = 3;
        [SerializeField] private Sprite[] upSprites;
        [SerializeField] private Sprite[] downSprites;
        [SerializeField] private Sprite[] leftSprites;

        private int spriteIdex;
        private float timeSenceLastFrame;
        private Vector3 moveDirection;
        private Vector3 lastMoveDirection;
        private new Rigidbody2D rigidbody;
        private SpriteRenderer spriteRenderer;

        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            rigidbody = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            moveDirection = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
            {
                moveDirection = Vector3.up;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                moveDirection = Vector3.down;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                moveDirection = Vector3.left;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                moveDirection = Vector3.right;
            }
            Animate();
            lastMoveDirection = moveDirection;
        }

        private void FixedUpdate()
        {
            rigidbody.velocity = moveDirection.normalized * moveSpeed;
        }

        private void Animate()
        {
            if (moveDirection != Vector3.zero)
            {
                timeSenceLastFrame -= Time.deltaTime;
                if (timeSenceLastFrame <= 0)
                {
                    if (spriteIdex == 3) spriteIdex = 1;
                    if (moveDirection == Vector3.up)
                    {
                        spriteRenderer.flipX = false;
                        spriteRenderer.sprite = upSprites[spriteIdex];
                    }
                    else if (moveDirection == Vector3.down)
                    {
                        spriteRenderer.flipX = false;
                        spriteRenderer.sprite = downSprites[spriteIdex];
                    }
                    else if (moveDirection == Vector3.left)
                    {
                        spriteRenderer.flipX = false;
                        spriteRenderer.sprite = leftSprites[spriteIdex];
                    }
                    else if (moveDirection == Vector3.right)
                    {
                        spriteRenderer.flipX = true;
                        spriteRenderer.sprite = leftSprites[spriteIdex];
                    }
                    spriteIdex++;
                    timeSenceLastFrame = timeTillNextFrame;
                }
                else if (lastMoveDirection != moveDirection)
                {
                    if (spriteIdex == 3) spriteIdex = 1;
                    if (moveDirection == Vector3.up)
                    {
                        spriteRenderer.flipX = false;
                        spriteRenderer.sprite = upSprites[spriteIdex];
                    }
                    else if (moveDirection == Vector3.down)
                    {
                        spriteRenderer.flipX = false;
                        spriteRenderer.sprite = downSprites[spriteIdex];
                    }
                    else if (moveDirection == Vector3.left)
                    {
                        spriteRenderer.flipX = false;
                        spriteRenderer.sprite = leftSprites[spriteIdex];
                    }
                    else if (moveDirection == Vector3.right)
                    {
                        spriteRenderer.flipX = true;
                        spriteRenderer.sprite = leftSprites[spriteIdex];
                    }
                    spriteIdex++;
                    timeSenceLastFrame = timeTillNextFrame;
                }
            }
            else
            {
                if (lastMoveDirection == Vector3.up)
                {
                    spriteRenderer.flipX = false;
                    spriteRenderer.sprite = upSprites[0];
                }
                else if (lastMoveDirection == Vector3.down)
                {
                    spriteRenderer.flipX = false;
                    spriteRenderer.sprite = downSprites[0];
                }
                else if (lastMoveDirection == Vector3.left)
                {
                    spriteRenderer.flipX = false;
                    spriteRenderer.sprite = leftSprites[0];
                }
                else if (lastMoveDirection == Vector3.right)
                {
                    spriteRenderer.flipX = true;
                    spriteRenderer.sprite = leftSprites[0];
                }
                spriteIdex = 1;
            }
        }

    }
}
