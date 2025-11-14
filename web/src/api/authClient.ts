/**
 * Authentication and Webhook Management API Client
 * Uses httpOnly cookies for authentication
 */

import { API_BASE_URL } from './config';

/**
 * Type Definitions
 */

export interface UserInfo {
  id: number;
  discord_user_id: string;
  discord_username: string;
  discord_avatar?: string;
  registered_at: string;
}

export interface Webhook {
  id: number;
  owner_user_id?: number;
  url: string;
  created_at: string;
  subscribe_jp: boolean;
  subscribe_kr: boolean;
  subscribe_cn: boolean;
}

export interface WebhooksResponse {
  webhooks: Webhook[];
  total: number;
}

export interface CreateWebhookRequest {
  url: string;
  subscribe_jp: boolean;
  subscribe_kr: boolean;
  subscribe_cn: boolean;
}

export interface UpdateWebhookRequest {
  url?: string;
  subscribe_jp?: boolean;
  subscribe_kr?: boolean;
  subscribe_cn?: boolean;
}

/**
 * Base fetch wrapper with cookie credentials
 */
async function authFetch<T>(
  endpoint: string,
  options: RequestInit = {}
): Promise<T> {
  const url = `${API_BASE_URL}${endpoint}`;

  const headers: HeadersInit = {
    'Content-Type': 'application/json',
    ...options.headers,
  };

  try {
    const response = await fetch(url, {
      ...options,
      headers,
      credentials: 'include', // Send cookies with requests
    });

    if (response.status === 204) {
      return {} as T;
    }

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(`API request failed: ${response.status} ${errorText}`);
    }

    return await response.json();
  } catch (error) {
    console.error(`Error fetching ${url}:`, error);
    throw error;
  }
}

/**
 * Auth API Functions
 */

/**
 * Get current user info (checks if logged in via cookie)
 */
export async function getCurrentUser(): Promise<UserInfo | null> {
  try {
    return await authFetch<UserInfo>('/auth/me');
  } catch (error) {
    // User not authenticated
    return null;
  }
}

/**
 * Logout the current user
 */
export async function logout(): Promise<void> {
  await authFetch<void>('/auth/logout', {
    method: 'POST',
  });
}

/**
 * Webhook API Functions
 */

/**
 * List all webhooks for the current user
 */
export async function listWebhooks(): Promise<WebhooksResponse> {
  return authFetch<WebhooksResponse>('/user/webhooks');
}

/**
 * Create a new webhook
 */
export async function createWebhook(webhook: CreateWebhookRequest): Promise<Webhook> {
  return authFetch<Webhook>('/user/webhooks', {
    method: 'POST',
    body: JSON.stringify(webhook),
  });
}

/**
 * Get a specific webhook
 */
export async function getWebhook(id: number): Promise<Webhook> {
  return authFetch<Webhook>(`/user/webhooks/${id}`);
}

/**
 * Update a webhook
 */
export async function updateWebhook(
  id: number,
  webhook: UpdateWebhookRequest
): Promise<Webhook> {
  return authFetch<Webhook>(`/user/webhooks/${id}`, {
    method: 'PATCH',
    body: JSON.stringify(webhook),
  });
}

/**
 * Delete a webhook
 */
export async function deleteWebhook(id: number): Promise<void> {
  return authFetch<void>(`/user/webhooks/${id}`, {
    method: 'DELETE',
  });
}

/**
 * Test a webhook by sending sample data
 */
export async function testWebhook(id: number): Promise<any> {
  return authFetch<any>(`/user/webhooks/${id}/test`, {
    method: 'POST',
  });
}
