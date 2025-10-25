using NUnit.Framework;
using UnityEngine;
using GorgonzolaMM;

namespace GorgonzolaMM.Tests
{
    /// <summary>
    /// Health System Tests.
    /// Validates damage application, death triggering, and respawn state.
    /// Critical for win/lose condition validation.
    /// </summary>
    [Category("Health")]
    public class HealthSystemTests
    {
        private GameObject entityGO;
        
        [SetUp]
        public void Setup()
        {
            entityGO = new GameObject("TestEntity");
        }

        [TearDown]
        public void Teardown()
        {
            Object.Destroy(entityGO);
        }

        [Test]
        public void Health_CanBeAdded_ToGameObject()
        {
            // Add a basic Health-like component
            // (TopDown Engine's Health component, but we'll mock behavior)
            var health = entityGO.AddComponent<SimpleHealthMock>();
            
            Assert.IsNotNull(health);
            Assert.AreEqual(100, health.CurrentHealth);
            Debug.Log("[Test] ✓ Health component can be added");
        }

        [Test]
        public void Health_ApplyDamage_ReducesHealth()
        {
            var health = entityGO.AddComponent<SimpleHealthMock>();
            int initialHealth = health.CurrentHealth;
            
            health.ApplyDamage(10);
            
            Assert.AreEqual(initialHealth - 10, health.CurrentHealth);
            Debug.Log($"[Test] ✓ Damage applied: {initialHealth} → {health.CurrentHealth}");
        }

        [Test]
        public void Health_ApplyDamage_ClampsToZero()
        {
            var health = entityGO.AddComponent<SimpleHealthMock>();
            
            health.ApplyDamage(200); // More than max
            
            Assert.That(health.CurrentHealth, Is.GreaterThanOrEqualTo(0));
            Debug.Log("[Test] ✓ Health clamped to >= 0");
        }

        [Test]
        public void Health_IsDead_WhenHealthZero()
        {
            var health = entityGO.AddComponent<SimpleHealthMock>();
            
            Assert.IsFalse(health.IsDead, "Should not be dead initially");
            
            health.ApplyDamage(200);
            
            Assert.IsTrue(health.IsDead, "Should be dead when health = 0");
            Debug.Log("[Test] ✓ Death state triggered correctly");
        }

        [Test]
        public void Health_FiresEvent_OnDeath()
        {
            var health = entityGO.AddComponent<SimpleHealthMock>();
            bool deathEventFired = false;
            
            health.OnDeath += () => deathEventFired = true;
            health.ApplyDamage(200);
            
            Assert.IsTrue(deathEventFired, "OnDeath event should fire");
            Debug.Log("[Test] ✓ OnDeath event fires when health = 0");
        }

        [Test]
        public void Health_FiresEvent_OnDamageApplied()
        {
            var health = entityGO.AddComponent<SimpleHealthMock>();
            bool damageEventFired = false;
            int damageAmount = 0;
            
            health.OnDamageApplied += (amount) => 
            { 
                damageEventFired = true;
                damageAmount = amount;
            };
            
            health.ApplyDamage(15);
            
            Assert.IsTrue(damageEventFired, "OnDamageApplied event should fire");
            Assert.AreEqual(15, damageAmount, "Event should pass correct damage amount");
            Debug.Log("[Test] ✓ OnDamageApplied event fires with correct amount");
        }

        [Test]
        public void Health_Heal_IncreasesHealth()
        {
            var health = entityGO.AddComponent<SimpleHealthMock>();
            health.ApplyDamage(50);
            int damagedHealth = health.CurrentHealth;
            
            health.Heal(25);
            
            Assert.AreEqual(damagedHealth + 25, health.CurrentHealth);
            Debug.Log($"[Test] ✓ Healing works: {damagedHealth} → {health.CurrentHealth}");
        }

        [Test]
        public void Health_Heal_ClampsToMax()
        {
            var health = entityGO.AddComponent<SimpleHealthMock>();
            int maxHealth = health.MaxHealth;
            
            health.Heal(500); // More than max
            
            Assert.AreEqual(maxHealth, health.CurrentHealth);
            Debug.Log("[Test] ✓ Health clamped to max on heal");
        }

        [Test]
        public void Health_CanBeReset_ToMax()
        {
            var health = entityGO.AddComponent<SimpleHealthMock>();
            health.ApplyDamage(75);
            
            health.Reset();
            
            Assert.AreEqual(health.MaxHealth, health.CurrentHealth);
            Assert.IsFalse(health.IsDead);
            Debug.Log("[Test] ✓ Health reset to max");
        }
    }

    /// <summary>
    /// Simple mock of TopDown Engine's Health component for testing.
    /// When you integrate with actual TopDown Engine, replace with real Health tests.
    /// </summary>
    public class SimpleHealthMock : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 100;
        private int currentHealth;

        public int MaxHealth => maxHealth;
        public int CurrentHealth => currentHealth;
        public bool IsDead => currentHealth <= 0;

        public event System.Action OnDeath;
        public event System.Action<int> OnDamageApplied;

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        public void ApplyDamage(int amount)
        {
            if (amount < 0) amount = 0;
            
            currentHealth -= amount;
            if (currentHealth < 0) currentHealth = 0;
            
            OnDamageApplied?.Invoke(amount);
            
            if (IsDead)
            {
                OnDeath?.Invoke();
            }
        }

        public void Heal(int amount)
        {
            if (amount < 0) amount = 0;
            
            currentHealth += amount;
            if (currentHealth > maxHealth) currentHealth = maxHealth;
        }

        public void Reset()
        {
            currentHealth = maxHealth;
        }
    }
}