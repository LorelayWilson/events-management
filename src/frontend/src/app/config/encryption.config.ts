/**
 * Encryption configuration for secure storage
 * 
 * IMPORTANT: In production, this key must be:
 * 1. Generated securely
 * 2. Stored in environment variables
 * 3. Different for each environment (development, staging, production)
 * 4. Rotated periodically
 */

export const ENCRYPTION_CONFIG = {
  // Encryption key - CHANGE IN PRODUCTION
  SECRET_KEY: 'your-super-secret-encryption-key-change-in-production-2024',
  
  // Additional configuration for enhanced security
  ALGORITHM: 'AES',
  KEY_SIZE: 256,
  
  // Prefix to identify encrypted values
  ENCRYPTED_PREFIX: 'enc_'
};

/**
 * Generates a secure encryption key
 * Use this function to generate new keys in production
 */
export function generateSecureKey(): string {
  const array = new Uint8Array(32);
  crypto.getRandomValues(array);
  return Array.from(array, byte => byte.toString(16).padStart(2, '0')).join('');
} 