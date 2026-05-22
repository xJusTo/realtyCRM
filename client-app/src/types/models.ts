export type PropertyType = 0 | 1 | 2;
export const PropertyType = {
    Apartment: 0 as const,
    House: 1 as const,
    Commercial: 2 as const
};

export type PropertyStatus = 0 | 1 | 2;
export const PropertyStatus = {
    Available: 0 as const,
    Booked: 1 as const,
    Sold: 2 as const
};


export interface Property {
    id: number;
    address: string;
    description: string;
    price: number;
    area: number;
    type: PropertyType;
    status: PropertyStatus;
    realtorId: number;
    photoUrl: string;
}

export interface User {
    id: number;
    fullName: string;
    phone: string;
    email: string;
    role: 'Client' | 'Realtor';
    preferences?: string;
    commissionRate?: number;
}

export interface Booking {
    id: number;
    propertyId: number;
    clientName: string;
    clientPhone: string;
    clientEmail: string;
    bookingDate: string;
    status: 'Pending' | 'Approved' | 'Rejected';
}

export interface Deal {
    id: number;
    propertyId: number;
    clientId: number;
    realtorId: number;
    dealDate: string;
    finalPrice: number;
}
