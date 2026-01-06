export interface ApiResponse<T = any> {
  succeeded: boolean;
  data?: T;
  error?: string;
  errors?: string[];
  statusCode?: number;
  timestamp?: Date;
}

export interface ApiError {
  statusCode: number;
  message: string;
  timestamp: Date;
}
