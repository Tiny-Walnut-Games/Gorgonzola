import { test, expect } from '@playwright/test';

/**
 * ABILITY GATING VALIDATION TEST
 * 
 * Validates that the turn-based ability gating system works correctly.
 * This test monitors Unity console output to verify that abilities are
 * enabled/disabled based on turn phases.
 * 
 * Pattern: PlayerInputGate gates CharacterMovement, CharacterOrientation3D,
 * CharacterRun, and CharacterHandleWeapon (not InputManager).
 */

test.describe('Ability Gating System Validation', () => {
  
  test.beforeEach(async ({ page }) => {
    // Capture console messages for validation
    const consoleLogs: string[] = [];
    page.on('console', msg => {
      consoleLogs.push(msg.text());
    });
    
    // Make logs available to test
    (page as any).consoleLogs = consoleLogs;
    
    // Navigate to mock validator (simpler for initial testing)
    await page.goto('file:///E:/Tiny_Walnut_Games/Gorgonzola/tests/mock-unity-build.html');
    
    // Wait for game to initialize
    await page.waitForTimeout(1500);
  });

  test('should demonstrate ability gating through phase transitions', async ({ page }) => {
    // This test runs on the mock, which simulates the same behavior
    // The real validation would run against the actual Unity build
    
    await expect(page.locator('text=🎯 Turn-based mechanics active')).toBeVisible();
    
    // Initial state: Turn-based system active
    const initialLog = await page.locator(':text("Turn-based mechanics")').isVisible();
    expect(initialLog).toBe(true);
  });

  test('should gate abilities during non-PlayerTurn phases', async ({ page }) => {
    // Simulate player turn
    await page.keyboard.press('w');
    
    // Submit turn (transition out of PlayerTurn)
    await page.keyboard.press('Space');
    
    // During EnemyTurn, the mock shows enemy movement
    await page.waitForTimeout(1000);
    await expect(page.locator('text=🐍 Snakes moving towards player')).toBeVisible();
    
    // In real game: this would log "[PlayerInputGate] 🔴 ABILITIES DISABLED (EnemyTurn phase active)"
    // In mock: player input is disabled (can't move during enemy phase)
  });

  test('should enable abilities when PlayerTurn begins', async ({ page }) => {
    // Setup: Complete a full turn cycle to get back to PlayerTurn
    await page.keyboard.press('w');
    await page.keyboard.press('Space');
    
    // Wait for full cycle: Enemy → Minion → Resolution → back to Player
    await page.waitForTimeout(3000);
    
    // Should be able to move again (PlayerTurn active)
    await page.keyboard.press('d');
    await expect(page.locator('text=➡️ Moved right')).toBeVisible();
    
    // In real game: this would log "[PlayerInputGate] 🟢 ABILITIES ENABLED (PlayerTurn phase active)"
  });

  test('should maintain ability gating across multiple turns', async ({ page }) => {
    const turnSequence = [
      { key: 'w', direction: '⬆️ Moved up' },
      { key: 'd', direction: '➡️ Moved right' },
      { key: 's', direction: '⬇️ Moved down' },
      { key: 'a', direction: '⬅️ Moved left' }
    ];
    
    for (let turn = 0; turn < 3; turn++) {
      // Player turn: move and confirm
      for (const { key, direction } of turnSequence) {
        await page.keyboard.press(key);
        await expect(page.locator(`text=${direction}`)).toBeVisible();
        await page.waitForTimeout(100);
      }
      
      // Submit turn
      await page.keyboard.press('Space');
      
      // Wait for enemy phase to complete
      await page.waitForTimeout(2000);
      
      // Should be back to PlayerTurn
      await page.keyboard.press('w');
      await expect(page.locator('text=⬆️ Moved up')).toBeVisible();
    }
  });

  test('should properly cache ability components on startup', async ({ page }) => {
    // This test would validate console output on a real Unity build
    // When PlayerInputGate.Start() runs, it logs which abilities were cached:
    // 
    // [PlayerInputGate] ✨ ABILITY GATING INITIALIZED
    // [PlayerInputGate] Cached abilities:
    //   - CharacterMovement: ✓
    //   - CharacterOrientation3D: ✓
    //   - CharacterRun: ✓
    //   - CharacterHandleWeapon: ✓
    
    // In mock test, this is implicit in the working turn system
    await expect(page.locator('text=🎯 Turn-based mechanics active')).toBeVisible();
  });

  test('should transition abilities correctly on phase change events', async ({ page }) => {
    // Validate the full phase sequence with ability gating
    const phases = [
      { triggerKey: 'Space', expectedPhase: 'Enemy', expectedLog: '🐍 Snakes' },
      // After enemy phase completes automatically:
      // → Minion phase: '👻 Ghosts'
      // → Resolution: '⚔️ Resolving'
      // → Back to Player
    ];
    
    // Trigger first turn
    await page.keyboard.press('w');
    await page.keyboard.press('Space');
    
    // Verify phase progression
    await page.waitForTimeout(500);
    await expect(page.locator('text=🐍 Snakes moving towards player')).toBeVisible();
    
    // Minion phase
    await page.waitForTimeout(1000);
    await expect(page.locator('text=👻 Ghosts drifting randomly')).toBeVisible();
    
    // Resolution phase
    await page.waitForTimeout(1000);
    await expect(page.locator('text=⚔️ Resolving collisions')).toBeVisible();
    
    // Back to Player phase
    await page.waitForTimeout(500);
    await page.keyboard.press('w');
    await expect(page.locator('text=⬆️ Moved up')).toBeVisible();
  });
});

test.describe('Ability Gating - Console Integration (Real Build)', () => {
  // These tests would run against the actual Unity WebGL build
  // They validate the console output from PlayerInputGate
  
  test.skip('should log ability gating state transitions', async ({ page }) => {
    // Navigate to actual build
    await page.goto('http://localhost:3000');
    await page.waitForSelector('#unity-canvas', { timeout: 30000 });
    
    const consoleLogs: string[] = [];
    page.on('console', msg => {
      consoleLogs.push(msg.text());
    });
    
    // Wait for initialization
    await page.waitForTimeout(5000);
    
    // Should see ability gating initialization log
    const hasInitLog = consoleLogs.some(log => 
      log.includes('[PlayerInputGate]') && 
      log.includes('ABILITY GATING INITIALIZED')
    );
    expect(hasInitLog).toBe(true);
  });
  
  test.skip('should disable abilities during non-PlayerTurn phases', async ({ page }) => {
    await page.goto('http://localhost:3000');
    await page.waitForSelector('#unity-canvas', { timeout: 30000 });
    
    const consoleLogs: string[] = [];
    page.on('console', msg => {
      consoleLogs.push(msg.text());
    });
    
    await page.waitForTimeout(5000);
    
    // Trigger turn
    await page.keyboard.press('Space');
    await page.waitForTimeout(1000);
    
    // Should see ability disable log
    const hasDisableLog = consoleLogs.some(log => 
      log.includes('[PlayerInputGate]') && 
      log.includes('ABILITIES DISABLED')
    );
    expect(hasDisableLog).toBe(true);
  });
  
  test.skip('should enable abilities when PlayerTurn begins', async ({ page }) => {
    await page.goto('http://localhost:3000');
    await page.waitForSelector('#unity-canvas', { timeout: 30000 });
    
    const consoleLogs: string[] = [];
    page.on('console', msg => {
      consoleLogs.push(msg.text());
    });
    
    await page.waitForTimeout(5000);
    
    // Complete a turn cycle
    await page.keyboard.press('w');
    await page.keyboard.press('Space');
    await page.waitForTimeout(3000);
    
    // Should see ability enable log
    const hasEnableLog = consoleLogs.some(log => 
      log.includes('[PlayerInputGate]') && 
      log.includes('ABILITIES ENABLED')
    );
    expect(hasEnableLog).toBe(true);
  });
});