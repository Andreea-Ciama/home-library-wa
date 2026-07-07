export interface ApiResponse<T> {
  success: boolean;
  errors: string[];
  data: T | null;
}

export interface ImportResult {
  queued: number;
  message: string;
}
