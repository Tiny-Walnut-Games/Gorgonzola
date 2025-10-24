import { chromium, FullConfig } from '@playwright/test';

/**
 * Global setup for Unity WebGL E2E tests
 * Ensures Unity build is ready and responsive before tests begin
 */
async function globalSetup(config: FullConfig) {
  console.log('🎮 Setting up Unity WebGL testing environment...');
  
  // Launch browser to verify Unity build is accessible
  const browser = await chromium.launch();
  const page = await browser.newPage();
  
  try {
    console.log('📡 Checking Unity WebGL build availability...');
    await page.goto('http://localhost:3000', { 
      waitUntil: 'networkidle',
      timeout: 30000 
    });
    
    // Wait for Unity container to be present
    await page.waitForSelector('#unity-container', { timeout: 30000 });
    
    // Wait for Unity to initialize (this might take time for WebGL builds)
    console.log('⚙️ Waiting for Unity to initialize...');
    await page.waitForTimeout(10000);
    
    // Check if Unity canvas is ready
    const canvasReady = await page.locator('#unity-canvas').isVisible();
    if (!canvasReady) {
      throw new Error('Unity canvas not ready');
    }
    
    console.log('✅ Unity WebGL build is ready for testing');
    
  } catch (error) {
    console.error('❌ Failed to initialize Unity WebGL build:', error);
    throw error;
  } finally {
    await browser.close();
  }
}

export default globalSetup;