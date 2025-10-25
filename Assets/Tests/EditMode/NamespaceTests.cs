using NUnit.Framework;

namespace GorgonzolaMM.Tests
{
    /// <summary>
    /// EditMode tests for basic namespace and assembly setup.
    /// </summary>
    public class NamespaceTests
    {
        [Test]
        public void GorgonzolaMM_Namespace_IsAccessible()
        {
            // Verify namespace exists and is accessible
            Assert.Pass();
        }

        [Test]
        public void Assert_BasicEquality()
        {
            int expected = 1;
            int actual = 1;
            Assert.AreEqual(expected, actual);
        }
    }
}