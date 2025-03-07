using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

namespace Lovatto.MobileInput
{
    public class Target : MonoBehaviour
    {
        public Transform cameraTransform;
        
        public float health = 5.0f;
        public int pointValue;

        public ParticleSystem DestroyedEffect;

        [Header("Audio")]
        public RandomPlayer HitPlayer;
        public AudioSource IdleSource;

        public bool Destroyed => m_Destroyed;

        bool m_Destroyed = false;
        float m_CurrentHealth;

        void Awake()
        {
            Helpers.RecursiveLayerChange(transform, LayerMask.NameToLayer("Target"));
        }

        void Start()
        {
            if (DestroyedEffect)
                PoolSystem.Instance.InitPool(DestroyedEffect, 16);

            m_CurrentHealth = health;
            if (IdleSource != null)
                IdleSource.time = Random.Range(0.0f, IdleSource.clip.length);
        }

        private void OnEnable()
        {
            cameraTransform = Camera.main.transform;
            this.GetComponent<AIDestinationSetter>().target = cameraTransform;

            this.transform.Find("MinimapSprite").gameObject.layer = 3;
            //MoveToPlayer();
        }

        void Update()
        {
            if (Vector2.Distance(new Vector2(this.transform.position.x, this.transform.position.z), new Vector2(cameraTransform.position.x, cameraTransform.position.z )) < 0.5f)
            {
                DamagePlayer();
            }
        }

        public void Got(float damage)
        {
            m_CurrentHealth -= damage;

            if (HitPlayer != null)
                HitPlayer.PlayRandom();

            if (m_CurrentHealth > 0)
                return;

            Vector3 position = HitPlayer.transform.position;

            //the audiosource of the target will get destroyed, so we need to grab a world one and play the clip through it
            if (HitPlayer != null)
            {
                var source = WorldAudioPool.GetWorldSFXSource();
                source.transform.position = position;
                source.pitch = HitPlayer.source.pitch;
                source.PlayOneShot(HitPlayer.GetRandomClip());
            }

            if (DestroyedEffect != null)
            {
                var effect = PoolSystem.Instance.GetInstance<ParticleSystem>(DestroyedEffect);
                effect.time = 0.0f;
                effect.Play();
                effect.transform.position = position;
            }

            m_Destroyed = true;
            
            GameInfoManager.Instance.Kill();
            gameObject.SetActive(false);
        }

        private void DamagePlayer()
        {
            Vector3 position = HitPlayer.transform.position;
            
            if (DestroyedEffect != null)
            {
                var effect = PoolSystem.Instance.GetInstance<ParticleSystem>(DestroyedEffect);
                effect.time = 0.0f;
                effect.Play();
                effect.transform.position = position;
            }

            m_Destroyed = true;

            gameObject.SetActive(false);
            
            GameInfoManager.Instance.GotDamage();
        }

        private void MoveToPlayer()
        {
            StartCoroutine(CoMove());
        }
        private IEnumerator CoMove()
        {
            WaitForEndOfFrame frame = new WaitForEndOfFrame();

            while (!m_Destroyed)
            {
                this.transform.LookAt(cameraTransform.position);
                
                Vector3 moveVector = cameraTransform.position - this.transform.position;
                moveVector = (moveVector - new Vector3(0, moveVector.y, 0)).normalized;

                this.transform.position += moveVector * Time.deltaTime / 8.0f;

                if (Vector2.Distance(new Vector2(this.transform.position.x, this.transform.position.z), new Vector2(cameraTransform.position.x, cameraTransform.position.z )) < 0.5f)
                {
                    DamagePlayer();
                }
                yield return frame;
            }
        }
    }
}