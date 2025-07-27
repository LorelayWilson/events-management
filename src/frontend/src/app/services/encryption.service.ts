import { Injectable } from '@angular/core';
import * as CryptoJS from 'crypto-js';
import { ENCRYPTION_CONFIG } from '../config/encryption.config';

@Injectable({
  providedIn: 'root'
})
export class EncryptionService {

  constructor() {}

  /**
   * Encrypts a value using AES-256
   */
  encrypt(value: string): string {
    try {
      return CryptoJS.AES.encrypt(value, ENCRYPTION_CONFIG.SECRET_KEY).toString();
    } catch (error) {
      console.error('Error encrypting:', error);
      throw new Error('Error encrypting the value');
    }
  }

  /**
   * Decrypts a value using AES-256
   */
  decrypt(encryptedValue: string): string {
    try {
      const bytes = CryptoJS.AES.decrypt(encryptedValue, ENCRYPTION_CONFIG.SECRET_KEY);
      const decrypted = bytes.toString(CryptoJS.enc.Utf8);
      
      if (!decrypted) {
        throw new Error('Decrypted value is empty');
      }
      
      return decrypted;
    } catch (error) {
      console.error('Error decrypting:', error);
      throw new Error('Error decrypting the value');
    }
  }

  /**
   * Saves an encrypted value to localStorage
   */
  setEncryptedItem(key: string, value: string): void {
    try {
      const encryptedValue = this.encrypt(value);
      const prefixedKey = `${ENCRYPTION_CONFIG.ENCRYPTED_PREFIX}${key}`;
      localStorage.setItem(prefixedKey, encryptedValue);
    } catch (error) {
      console.error('Error saving encrypted value:', error);
      throw error;
    }
  }

  /**
   * Gets and decrypts a value from localStorage
   */
  getDecryptedItem(key: string): string | null {
    try {
      const prefixedKey = `${ENCRYPTION_CONFIG.ENCRYPTED_PREFIX}${key}`;
      const encryptedValue = localStorage.getItem(prefixedKey);
      
      if (!encryptedValue) {
        return null;
      }

      return this.decrypt(encryptedValue);
    } catch (error) {
      console.error('Error getting decrypted value:', error);
      // If there's a decryption error, remove the corrupted value
      this.removeEncryptedItem(key);
      return null;
    }
  }

  /**
   * Removes an encrypted value from localStorage
   */
  removeEncryptedItem(key: string): void {
    const prefixedKey = `${ENCRYPTION_CONFIG.ENCRYPTED_PREFIX}${key}`;
    localStorage.removeItem(prefixedKey);
  }

  /**
   * Checks if an encrypted value exists in localStorage
   */
  hasEncryptedItem(key: string): boolean {
    const prefixedKey = `${ENCRYPTION_CONFIG.ENCRYPTED_PREFIX}${key}`;
    return localStorage.getItem(prefixedKey) !== null;
  }

  /**
   * Clears all encrypted values from localStorage
   */
  clearAllEncryptedItems(): void {
    const keys = Object.keys(localStorage);
    keys.forEach(key => {
      if (key.startsWith(ENCRYPTION_CONFIG.ENCRYPTED_PREFIX)) {
        localStorage.removeItem(key);
      }
    });
  }
} 