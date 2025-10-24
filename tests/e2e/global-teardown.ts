import { FullConfig } from '@playwright/test';

/**
 * Global teardown for Unity WebGL E2E tests
 * Cleanup after test completion
 */
async function globalTeardown(config: FullConfig) {
  console.log('🧹 Cleaning up Unity WebGL testing environment...');
  
  // Any cleanup operations can go here
  // For Unity WebGL, usually no special cleanup is needed
  
  console.log('✅ Test environment cleanup completed');
}

export default globalTeardown;